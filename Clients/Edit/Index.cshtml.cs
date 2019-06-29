using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Clients.Models;
using EventHorizon.Identity.AuthServer.Configuration;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Identity.AuthServer.Clients.Edit
{
    public class IndexModel : PageModel
    {
        readonly HistoryExtendedConfigurationDbContext _configurationDbContext;
        public IndexModel(
            HistoryExtendedConfigurationDbContext configurationDbContext
        )
        {
            _configurationDbContext = configurationDbContext;
        }


        [BindProperty]
        public Client Entity { get; set; }
        [BindProperty]
        public string ClientId { get; set; }
        [Display(Name = "Client Name")]
        [BindProperty]
        public string ClientName { get; set; }
        [Display(Name = "Front Channel Logout Uri")]
        [BindProperty]
        public string FrontChannelLogoutUri { get; set; }
        [BindProperty]
        public bool AllowOfflineAccess { get; set; }
        [BindProperty]
        public bool AllowAccessTokensViaBrowser { get; set; }
        [BindProperty]
        public bool RequireClientSecret { get; set; }
        public List<ApiResource> ApiResourceList { get; set; }

        public async Task OnGet(string id)
        {
            var entity = _configurationDbContext.Clients
                .Include("AllowedScopes")
                .Include("AllowedGrantTypes")
                .Include("ClientSecrets")
                .Include("RedirectUris")
                .Include("PostLogoutRedirectUris")
                .Include("AllowedCorsOrigins")
                .Include("AllowedScopes")
                .FirstOrDefault(a => a.ClientId == id);
            Entity = entity;
            ApiResourceList = await _configurationDbContext.ApiResources.ToListAsync();
            ClientId = id;

            AllowOfflineAccess = entity.AllowOfflineAccess;
            AllowAccessTokensViaBrowser = entity.AllowAccessTokensViaBrowser;
            RequireClientSecret = entity.RequireClientSecret;
        }

        public IActionResult OnPost(
            string id
        )
        {
            var entity = _configurationDbContext.Clients
                .FirstOrDefault(
                    a => a.ClientId == id
                );

            entity.ClientName =
                ClientName ?? entity.ClientName;
            entity.FrontChannelLogoutUri =
                FrontChannelLogoutUri ?? entity.FrontChannelLogoutUri;
            entity.AllowOfflineAccess =
                AllowOfflineAccess;
            entity.AllowAccessTokensViaBrowser =
                AllowAccessTokensViaBrowser;
            entity.RequireClientSecret =
                RequireClientSecret;

            _configurationDbContext.Clients.Update(entity);
            _configurationDbContext.SaveChanges();

            return RedirectToPage("./Index", new { Id = id });
        }
    }
}