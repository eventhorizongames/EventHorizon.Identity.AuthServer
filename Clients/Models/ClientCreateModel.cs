using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IdentityServer4.EntityFramework.Entities;

namespace EventHorizon.Identity.AuthServer.Clients.Models
{
    public class ClientCreateModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}