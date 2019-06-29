using System;
using System.Linq;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Clients.Extensions;
using EventHorizon.Identity.AuthServer.Clients.Models;
using EventHorizon.Identity.AuthServer.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventHorizon.Identity.AuthServer.Clients.Create
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
        public ClientCreateModel Client { get; set; }

        [BindProperty]
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (this.ClientAlreadyExists(
            ))
            {
                ModelState.AddModelError(
                    "duplicate", 
                    "client_already_exists"
                );
                return Page();
            }
            var entity = await _configurationDbContext.Clients.AddAsync(
                Client.ToEntity()
            );
            await _configurationDbContext.SaveChangesAsync();
            return RedirectToPage("../Index");
        }

        private bool ClientAlreadyExists()
        {
            return _configurationDbContext.Clients.Count(
                client => client.ClientId == Client.Id
            ) > 0;
        }
    }
}