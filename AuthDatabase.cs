using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using EventHorizon.Identity.AuthServer.Application;
using EventHorizon.Identity.AuthServer.Configuration;
using EventHorizon.Identity.AuthServer.Services.Claims;
using EventHorizon.Identity.AuthServer.Services.Role;
using EventHorizon.Identity.AuthServer.Services.User;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventHorizon.Identity.AuthServer
{
    public static class AuthDatabase
    {
        public static void InitializeDatabase(IServiceProvider services)
        {
            using (var serviceScope = services.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.MigrateContexts();
                serviceScope.CreateAdmins();
                serviceScope.CreateConfiguration();
            }
        }

        private static void MigrateContexts(this IServiceScope serviceScope)
        {
            var env = serviceScope.ServiceProvider.GetRequiredService<IHostingEnvironment>();
            if (env.IsDevelopment())
            {
                return;
            }
            serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>()
                .Database
                .Migrate();

            serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>()
                .Database
                .Migrate();

            serviceScope.ServiceProvider.GetRequiredService<HistoryExtendedConfigurationDbContext>()
                .Database
                .Migrate();

        }
        private static void CreateAdmins(this IServiceScope serviceScope)
        {
            var appContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var env = serviceScope.ServiceProvider.GetRequiredService<IHostingEnvironment>();
            var mediator = serviceScope.ServiceProvider.GetRequiredService<IMediator>();
            var config = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("admins.json")
                .AddJsonFile($"admins.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables()
                .Build();

            // Create Admin Roles
            mediator.Send(new RoleCreateEvent
            {
                RoleName = UserRoles.ADMIN
            }).GetAwaiter().GetResult();
            mediator.Send(new RoleAddClaimEvent
            {
                RoleName = UserRoles.ADMIN,
                Claim = new Claim(IdentityClaimTypes.PERMISSION, "identity.create"),
            }).GetAwaiter().GetResult();
            mediator.Send(new RoleAddClaimEvent
            {
                RoleName = UserRoles.ADMIN,
                Claim = new Claim(IdentityClaimTypes.PERMISSION, "identity.update"),
            }).GetAwaiter().GetResult();
            mediator.Send(new RoleAddClaimEvent
            {
                RoleName = UserRoles.ADMIN,
                Claim = new Claim(IdentityClaimTypes.PERMISSION, "identity.view"),
            }).GetAwaiter().GetResult();

            // Create Admins
            var admins = new AdminUserConfiguration();
            config.Bind(admins);
            foreach (var admin in admins.Admins)
            {
                var adminUser = appContext.Users.FirstOrDefault(a => a.Email == admin.Email);
                if (adminUser == null)
                {
                    var result = mediator.Send(new UserCreateEvent
                    {
                        User = new Models.ApplicationUser
                        {
                            UserName = admin.Email,
                            Email = admin.Email,
                        },
                        Profile = new Models.ApplicationUserProfile
                        {

                        },
                        Password = admin.Password,
                    }).GetAwaiter().GetResult();
                    if (result.Succeeded)
                    {
                        adminUser = appContext.Users.FirstOrDefault(a => a.Email == admin.Email);
                    }
                    else
                    {
                        continue;
                    }
                }
                mediator.Publish(new UserAddToRoleEvent
                {
                    User = adminUser,
                    Role = UserRoles.ADMIN
                }).GetAwaiter().GetResult();
            }

        }

        private class AdminUserConfiguration
        {
            public List<AdminUser> Admins { get; set; }
        }
        private class AdminUser
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        private static void CreateConfiguration(this IServiceScope serviceScope)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<HistoryExtendedConfigurationDbContext>();
            if (!context.Clients.Any())
            {
                foreach (var client in GetClients())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in GetIdentityResources())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in GetApiResources())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
        }
        private static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[] {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        private static IEnumerable<ApiResource> GetApiResources()
        {
            return new ApiResource[]
            {
                new ApiResource("api1", "My API #1")
            };
        }

        private static IEnumerable<Client> GetClients()
        {
            // client credentials flow client
            var credentialsClient = new Client
            {
                ClientId = "client",
                ClientName = "Client Credentials Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                AllowedScopes = { "api1" }
            };
            // MVC client using hybrid flow
            var hybridClient = new Client
            {
                ClientId = "mvc",
                ClientName = "MVC Client",

                AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                RedirectUris = { "http://gameserver.com:5000/signin-oidc" },
                FrontChannelLogoutUri = "http://gameserver.com:5000/signout-oidc",
                PostLogoutRedirectUris = { "http://gamerserver.com:5000/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "api1" }
            };
            // MVC client using hybrid flow
            var implClient = new Client
            {
                ClientId = "mvc_impl",
                ClientName = "MVC Implicit Client",

                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,

                RedirectUris = { "http://regauthserver.com:5001/signin-oidc" },
                PostLogoutRedirectUris = { "http://regauthserver.com:5001/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile
                }
            };
            // SPA client using implicit flow
            var spaClient = new Client
            {
                ClientId = "spa",
                ClientName = "SPA Client",
                ClientUri = "http://identityserver.io",

                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,

                RedirectUris = {
                    "http://localhost:5002/index.html",
                    "http://localhost:5002/callback.html",
                    "http://localhost:5002/silent.html",
                    "http://localhost:5002/popup.html",
                },

                PostLogoutRedirectUris = { "http://localhost:5002/index.html" },
                AllowedCorsOrigins = { "http://localhost:5002" },

                AllowedScopes = { "openid", "profile", "api1" }
            };
            return new[]
            {
                credentialsClient,

                hybridClient,

                implClient,

                spaClient
            };
        }
    }
}