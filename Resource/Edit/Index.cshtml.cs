using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EventHorizon.Identity.AuthServer.Configuration;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Identity.AuthServer.Resource.Edit
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
        public ApiResource Entity { get; set; }
        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }
        [BindProperty]
        public string Description { get; set; }

        public void OnGet(
            string id
        )
        {
            Entity = _configurationDbContext.ApiResources
                .Include("Scopes")
                .Include("UserClaims")
                .FirstOrDefault(a => a.Name == id);
            Name = Entity.Name;
        }

        public IActionResult OnPost(
            string id
        )
        {
            var entity = _configurationDbContext.ApiResources
                .FirstOrDefault(
                    a => a.Name == Name
                );

            entity.DisplayName = DisplayName ?? entity.DisplayName;
            entity.Description = Description ?? entity.Description;

            _configurationDbContext.ApiResources
                .Update(
                    entity
                );
            _configurationDbContext.SaveChanges();

            return RedirectToPage("./Index", new { Id = id });
        }
    }
}