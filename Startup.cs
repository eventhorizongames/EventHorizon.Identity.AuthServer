using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using EventHorizon.Identity.AuthServer.Application;
using EventHorizon.Identity.AuthServer.Configuration;
using EventHorizon.Identity.AuthServer.Email.Api;
using EventHorizon.Identity.AuthServer.Email.State;
using EventHorizon.Identity.AuthServer.Models;
using EventHorizon.Identity.AuthServer.Services;
using EventHorizon.Identity.AuthServer.Services.Claims;
using EventHorizon.Identity.AuthServer.Services.Models;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventHorizon.Identity.AuthServer
{
    public class Startup
    {
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
            // Application Insights
            services.AddApplicationInsightsTelemetry(
                options => Configuration.GetSection(
                    "ApplicationInsights"
                ).Bind(
                    options
                )
            );
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);
            services.AddRazorPages()
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
            if (HostEnvironment.IsDevelopment())
            {
                identityServer.AddDeveloperSigningCredential();
            }
            else
            {
                identityServer.AddSigningCredential(
                    new X509Certificate2(
                        Path.Combine(Directory.GetCurrentDirectory(), "certificate.pfx"),
                        Configuration["IdentityServerKeyPassword"]
                    )
                );
            }

            services.AddAuthentication();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Identity Admin",
                    policy => policy.RequireClaim(IdentityClaimTypes.PERMISSION, "identity.view", "identity.create", "identity.update"));
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


            if (HostEnvironment.IsDevelopment())
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
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            AuthDatabase.InitializeDatabase(
                app.ApplicationServices,
                RunMigrations(
                    Configuration["ConnectionType"],
                    HostEnvironment.IsDevelopment()
                )
            );

            app.UseRouting();
            app.UseCors("default");
            app.UseIdentityServer();

            app.UseStaticFiles();

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