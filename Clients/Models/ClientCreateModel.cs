using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventHorizon.Identity.AuthServer.Clients.Models
{
    public class ClientCreateModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }

        public bool AllowAccessTokensViaBrowser { get; set; } = true;
        public bool RequireClientSecret { get; set; } = false;

        public IList<AddAllowedCorsOriginModel> AllowedCorsOriginList { get; set; } = new List<AddAllowedCorsOriginModel>();
        public IList<AddGrantModel> GrantList { get; set; } = new List<AddGrantModel>();
        public IList<AddPostLogoutRedirectUriModel> PostLogoutRedirectUriList { get; set; } = new List<AddPostLogoutRedirectUriModel>();
        public IList<AddRedirectUriModel> RedirectUriList { get; set; } = new List<AddRedirectUriModel>();
        public IList<AddScopeModel> ScopeList { get; set; } = new List<AddScopeModel>();
    }
}