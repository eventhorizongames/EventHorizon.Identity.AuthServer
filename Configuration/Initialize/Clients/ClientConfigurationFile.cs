using System.Collections.Generic;
using IdentityServer4.Models;

namespace EventHorizon.Identity.AuthServer.Configuration.Initialize.Clients
{
    public class ClientConfigurationFile
    {
        public List<ClientConfiguration> Clients { get; set; } = new List<ClientConfiguration>();
    }
    public class ClientConfiguration : Client
    {
        public ICollection<string> ClientSecretStrings { get; set; }
    }
}
