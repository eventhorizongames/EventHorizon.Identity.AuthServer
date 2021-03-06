using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using EventHorizon.Identity.AuthServer.Application;
using EventHorizon.Identity.AuthServer.Captcha.Api;
using EventHorizon.Identity.AuthServer.Captcha.Models;
using EventHorizon.Identity.AuthServer.Captcha.Service;
using EventHorizon.Identity.AuthServer.Configuration;
using EventHorizon.Identity.AuthServer.Email.Api;
using EventHorizon.Identity.AuthServer.Email.State;
using EventHorizon.Identity.AuthServer.Models;
using EventHorizon.Identity.AuthServer.Monitoring.Model;
using EventHorizon.Identity.AuthServer.Monitoring.Telemetry;
using EventHorizon.Identity.AuthServer.Services;
using EventHorizon.Identity.AuthServer.Services.Claims;
using EventHorizon.Identity.AuthServer.Services.Models;
using MediatR;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace EventHorizon.Identity.AuthServer
{
    public class Startup
    {
        private const string XForwardedPathBase = "X-Forwarded-PathBase";
        private const string XForwardedProto = "X-Forwarded-Proto";

        public Startup(
            IConfiguration configuration,
            IHostEnvironment hostEnvironment
        )
        {
            this.Configuration = configuration;
            this.HostEnvironment = hostEnvironment;
        }
        public IConfiguration Configuration { get; }
        public IHostEnvironment HostEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Setup Captcha
            services.AddSingleton(
                new CaptchaSettings(
                    Configuration["Captcha:SiteKey"],
                    Configuration["Captcha:Secret"]
                )
            ).AddHttpClient<CaptchaService, CaptchaServiceHttpClient>(
                configuration =>
                {
                    configuration.BaseAddress = new Uri(
                        Configuration["Captcha:ApiUrl"]
                    );
                }
            );

            // Application Insights
            services.AddApplicationInsightsTelemetry(
                options => Configuration.GetSection(
                    "ApplicationInsights"
                ).Bind(
                    options
                )
            ).Configure<MonitoringServerConfiguration>(options =>
                {
                    options.Host = Configuration["VIRTUAL_HOST"] ?? "unset";
                    options.ServerName = Configuration["ServerName"] ?? "Identity";
                }
            ).AddSingleton<ITelemetryInitializer, NodeNameFilter>();

            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);
            services.AddHttpContextAccessor()
                .AddRazorPages()
                .AddRazorPagesOptions(options =>
                {
                    options.RootDirectory = "/";
                    options.Conventions.AuthorizeFolder("/Clients", "Identity Admin");
                    options.Conventions.AuthorizeFolder("/Resource", "Identity Admin");
                    options.Conventions.AuthorizeFolder("/Users", "Identity Admin");
                    options.Conventions.AuthorizeFolder("/Diagnostics", "Identity Admin");
                    options.Conventions.AuthorizeFolder("/Manage");
                    options.Conventions.AuthorizeFolder("/Grants");
                    options.Conventions.AuthorizeFolder("/Consent");
                });

            var isMSSqlConnectionType = IsMSSQLConnectionType(Configuration["ConnectionType"]);
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                if (!isMSSqlConnectionType && HostEnvironment.IsDevelopment())
                {
                    options.UseInMemoryDatabase("development_db");
                }
                else
                {
                    options.UseSqlServer(connectionString);
                }
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var identityServer = services.AddIdentityServer(options =>
                {
                    options.IssuerUri = Configuration["IssuerUri"] ?? null;
                    options.UserInteraction.ErrorUrl = "/Error";
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                // this adds the config data from DB (clients, resources, CORS)
                .AddConfigurationStore<HistoryExtendedConfigurationDbContext>(options =>
                {
                    if (!isMSSqlConnectionType && HostEnvironment.IsDevelopment())
                    {
                        options.ConfigureDbContext = builder =>
                            builder.UseInMemoryDatabase("development_db");
                    }
                    else
                    {
                        options.ConfigureDbContext = builder =>
                            builder.UseSqlServer(connectionString,
                                sql => sql.MigrationsAssembly(migrationsAssembly));
                    }
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    if (!isMSSqlConnectionType && HostEnvironment.IsDevelopment())
                    {
                        options.ConfigureDbContext = builder =>
                            builder.UseInMemoryDatabase("development_db");
                    }
                    else
                    {
                        options.ConfigureDbContext = builder =>
                            builder.UseSqlServer(connectionString,
                                sql => sql.MigrationsAssembly(migrationsAssembly));
                    }
                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    // options.TokenCleanupInterval = 15; // interval in seconds. 15 seconds useful for debugging
                })
                .AddAspNetIdentity<ApplicationUser>();

            // Setup Forwared Header Options, injects the proxy configuration into known network and proxies.
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor
                    | ForwardedHeaders.XForwardedProto;
                if (!String.IsNullOrEmpty(Configuration["ProxyServer"]))
                {
                    options.KnownNetworks.Add(
                        new IPNetwork(
                            IPAddress.Parse(
                                Configuration["ProxyServer"]
                            ).MapToIPv6(),
                            104
                        )
                    );
                    options.KnownProxies.Add(
                        IPAddress.Parse(
                            Configuration["ProxyServer"]
                        ).MapToIPv6()
                    );
                }
            });
            var securityKey = default(SecurityKey);
            if (HostEnvironment.IsDevelopment())
            {
                identityServer.AddDeveloperSigningCredential();
            }
            else
            {
                var certificate = new X509Certificate2(
                    Path.Combine(Directory.GetCurrentDirectory(), "certificate.pfx"),
                    Configuration["IdentityServerKeyPassword"]
                );
                identityServer.AddSigningCredential(
                    certificate
                );
                securityKey = new X509SecurityKey(
                    certificate
                );
            }

            services.AddAuthentication()
                .AddJwtBearer(
                    JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.Authority = Configuration["Auth:Authority"];
                        options.Audience = Configuration["Auth:Audience"];
                        options.RequireHttpsMetadata = !HostEnvironment.IsDevelopment();

                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = HostEnvironment.IsDevelopment(),

                            ValidIssuer = Configuration["IssuerUri"],
                            ValidAudience = Configuration["Auth:Audience"],
                            IssuerSigningKey = securityKey,
                        };
                    }
                )
            ;

            services.AddAuthorization(options =>
            {
                var adminRoleName = "Admin";
                var clientIdClaimType = "client_id";
                var releaseClientId = Configuration["Release:ClientId"];

                options.AddPolicy("Identity Admin",
                    policy => policy.RequireClaim(
                        IdentityClaimTypes.PERMISSION,
                        "identity.view",
                        "identity.create",
                        "identity.update"
                    )
                );
                options.AddPolicy(
                    "SystemAdmin",
                    builder => builder
                        .RequireAuthenticatedUser()
                        .RequireAssertion(
                            context => context.User.IsInRole(
                                adminRoleName
                            ) || context.User.HasClaim(
                                clientIdClaimType,
                                releaseClientId
                            )
                        )
                );
            });
            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            if (string.IsNullOrEmpty(
                Configuration["Email:ApiKey"]
            ))
            {
                services.AddScoped<IEmailSender, SaveToFileEmailSender>();
            }
            else
            {
                services.AddScoped<IEmailSender, EmailSender>();
            }
            services.AddSingleton<EmailTemplateRepository, InMemoryEmailTemplateRepository>();
            services.Configure<AuthMessageSenderOptions>(
                Configuration.GetSection("Email")
            );
        }

        public void Configure(IApplicationBuilder app)
        {
            app.AddEmailExtensions();
            app.UseForwardedHeaders();
            if (HostEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            AuthDatabase.InitializeDatabase(
                app.ApplicationServices,
                RunMigrations(
                    Configuration["ConnectionType"],
                    HostEnvironment.IsDevelopment()
                )
            );

            app.UseStatusCodePagesWithReExecute("/StatusCode", "?code={0}");

            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("default");
            app.Use((context, next) =>
            {
                if (context.Request.Headers.TryGetValue(XForwardedPathBase, out StringValues pathBase))
                {
                    context.Request.PathBase = new PathString(pathBase);
                }
                if (context.Request.Headers.TryGetValue(XForwardedProto, out StringValues proto))
                {
                    context.Request.Scheme = proto;
                }

                return next();
            });

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }

        private static bool RunMigrations(
            string connectionType,
            bool isDevelopment
        )
        {
            return IsMSSQLConnectionType(
                connectionType
            ) || !isDevelopment;
        }
        private static bool IsMSSQLConnectionType(
            string connectionType
        )
        {
            return !String.IsNullOrEmpty(
                connectionType
            ) && "MSSQL".Equals(
                connectionType
            );
        }
    }
}
