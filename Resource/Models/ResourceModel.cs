using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IdentityServer4.EntityFramework.Entities;

namespace EventHorizon.Identity.AuthServer.Resource.Models
{
    public class ResourceModel
    {
        public ApiResource Entity { get; set; }
        public string Name { get; set; }
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }
}