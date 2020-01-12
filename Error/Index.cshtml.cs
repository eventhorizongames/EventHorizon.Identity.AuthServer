using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;

namespace EventHorizon.Identity.AuthServer.Error
{
    public class IndexModel : PageModel
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IHostEnvironment _environment;

        public IndexModel(
            IIdentityServerInteractionService interaction,
            IHostEnvironment environment
        )
        {
            _interaction = interaction;
            _environment = environment;
        }

        [BindProperty]
        public ErrorMessage Error { get; set; }

        public async Task OnGetAsync(string errorId)
        {
            // retrieve error details from identityserver
            var errorMessage = await _interaction.GetErrorContextAsync(errorId);
            if (errorMessage != null)
            {
                this.Error = errorMessage;

                if (!_environment.IsDevelopment())
                {
                    // only show in development
                    errorMessage.ErrorDescription = null;
                }
            }
        }
    }
}