using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Configuration.Initialize.Clients
{
    public struct InitializeClientsHandler : IRequestHandler<InitializeClientsCommand, bool>
    {
        readonly HistoryExtendedConfigurationDbContext _context;

        public InitializeClientsHandler(
            HistoryExtendedConfigurationDbContext context
        )
        {
            _context = context;
        }
        
        public Task<bool> Handle(InitializeClientsCommand request, CancellationToken cancellationToken)
        {
            if (!_context.Clients.Any())
            {
                foreach (var client in GetClients())
                {
                    _context.Clients.Add(client.ToEntity());
                }
                _context.SaveChanges();
            }
            return Task.FromResult(
                true
            );
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

                RedirectUris = { "http://localhost:5555/signin-oidc" },
                FrontChannelLogoutUri = "http://localhost:5555/signout-oidc",
                PostLogoutRedirectUris = { "http://localhost:5555/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "api1", "roles" }
            };
            // MVC client using hybrid flow
            var implClient = new Client
            {
                ClientId = "mvc_impl",
                ClientName = "MVC Implicit Client",

                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,

                RedirectUris = { "http://localhost.com:5001/signin-oidc" },
                PostLogoutRedirectUris = { "http://localhost.com:5001/signout-callback-oidc" },

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