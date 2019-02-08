using System.Linq;
using IdentityServer4.Models;

namespace EventHorizon.Identity.AuthServer.Configuration.Initialize.Clients
{
    public static class ClientConfigurationMapper
    {
        public static Client ToEntity(this ClientConfiguration config)
        {
            return new Client
            {
                ClientId = config.ClientId,
                ClientName = config.ClientName,
                ClientUri = config.ClientUri,

                ClientSecrets = config.ClientSecretStrings?.Select(
                    clientSecretString => new Secret(
                        clientSecretString.Sha256()
                    )
                ).ToList(),

                AllowedGrantTypes = config.AllowedGrantTypes,
                AllowedScopes = config.AllowedScopes,
                AllowAccessTokensViaBrowser = config.AllowAccessTokensViaBrowser,

                RedirectUris = config.RedirectUris,
                FrontChannelLogoutUri = config.FrontChannelLogoutUri,
                PostLogoutRedirectUris = config.PostLogoutRedirectUris,

                AllowOfflineAccess = config.AllowOfflineAccess,
                AllowedCorsOrigins = config.AllowedCorsOrigins
            };
        }
    }
}