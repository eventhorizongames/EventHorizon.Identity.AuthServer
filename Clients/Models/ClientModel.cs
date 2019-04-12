using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IdentityServer4.EntityFramework.Entities;

namespace EventHorizon.Identity.AuthServer.Clients.Models
{
    public class ClientModel
    {
        public Client Entity { get; set; }
        public List<ApiResource> ApiResourceList { get; set; }

        public string ClientId { get; set; }
        [Display(Name = "Client Name")]
        public string ClientName { get; set; }
        [Display(Name = "Front Channel Logout Uri")]
        public string FrontChannelLogoutUri { get; set; }
        public bool AllowOfflineAccess { get; set; }
        public bool AllowAccessTokensViaBrowser { get; set; }
        public bool RequireClientSecret { get; set; }
    }
}