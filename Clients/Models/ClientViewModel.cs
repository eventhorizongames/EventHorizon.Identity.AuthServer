using System.Collections.Generic;
using IdentityServer4.EntityFramework.Entities;

namespace EventHorizon.Identity.AuthServer.Clients.Models
{
    public class ClientViewModel
    {
        
        public IList<Client> Clients { get; } = new List<Client>();
    }
}