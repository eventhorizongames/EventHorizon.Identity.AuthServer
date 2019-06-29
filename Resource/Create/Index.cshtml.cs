using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Configuration;
using EventHorizon.Identity.AuthServer.Resource.Models;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventHorizon.Identity.AuthServer.Resource.Create
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

        [Required]
        [BindProperty]
        [Display(Name = "Name", Prompt = "Name")]
        public string Name { get; set; }
        [Required]
        [BindProperty]
        [Display(Name = "Display Name", Prompt = "Display Name")]
        public string DisplayName { get; set; }
        [BindProperty]
        public string ReturnUrl { get; set; }
        [BindProperty]
        public string ErrorMessage { get; set; }

        public void OnGet(
            string returnUrl
        )
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (this.ResourceAlreadyExists(
            ))
            {
                ModelState.AddModelError(
                    "duplicate",
                    "resource_already_exists"
                );
                return Page();
            }
            var entity = await _configurationDbContext.ApiResources
                .AddAsync(new ApiResource(
                    Name,
                    DisplayName
                ).ToEntity()
            );
            await _configurationDbContext.SaveChangesAsync();

            if (ReturnUrl != null)
            {
                return Redirect(ReturnUrl);
            }
            return RedirectToPage("../Index");
        }
        private bool ResourceAlreadyExists()
        {
            return _configurationDbContext.ApiResources.Count(
                resource => resource.Name == Name
            ) > 0;
        }
    }
}