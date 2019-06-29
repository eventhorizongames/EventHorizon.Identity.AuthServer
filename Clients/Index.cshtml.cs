using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Configuration;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventHorizon.Identity.AuthServer.Clients
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
        public IList<Client> Clients { get; private set; } = new List<Client>();

        public void OnGet()
        {
            Clients = new List<Client>(
                _configurationDbContext.Clients
            );
        }
    }
}