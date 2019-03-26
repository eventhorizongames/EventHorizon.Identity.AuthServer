using System.Linq;
using IdentityServer4.Models;

namespace EventHorizon.Identity.AuthServer.Configuration.Initialize.Clients
{
    public static class ClientConfigurationMapper
    {
        public static Client ToEntity(this ClientConfiguration config)
        {
            config.ClientSecrets = config.ClientSecretStrings?.Select(
                    clientSecretString => new Secret(
                        clientSecretString.Sha256()
                    )
                ).ToList();
            return config;
        }
    }
}