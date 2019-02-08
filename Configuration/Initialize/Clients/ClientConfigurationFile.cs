using System.Collections.Generic;

namespace EventHorizon.Identity.AuthServer.Configuration.Initialize.Clients
{
    public class ClientConfigurationFile
    {
        public List<ClientConfiguration> Clients { get; set; }
    }
    public class ClientConfiguration
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientUri { get; set; }
        public ICollection<string> AllowedGrantTypes { get; set; }
        public ICollection<string> ClientSecretStrings { get; set; }
        public ICollection<string> AllowedScopes { get; set; }
        public ICollection<string> RedirectUris { get; set; }
        public string FrontChannelLogoutUri { get; set; }
        public ICollection<string> PostLogoutRedirectUris { get; set; }
        public bool AllowOfflineAccess { get; set; }
        public bool AllowAccessTokensViaBrowser { get; set; }
        public ICollection<string> AllowedCorsOrigins { get; set; }
    }
}