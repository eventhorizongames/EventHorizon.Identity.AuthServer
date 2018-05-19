using System.Linq;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Clients.Models;
using EventHorizon.Identity.AuthServer.Configuration;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Identity.AuthServer.Clients
{
    public partial class ClientController : Controller
    {
        [HttpPost("{clientId}/removeRedirectUri/{id:int}")]
        public async Task<IActionResult> RemoveRedirectUri(string clientId, int id)
        {
            var uri = _configurationDbContext.ClientRedirectUris
                .FirstOrDefault(a => a.Client.ClientId == clientId && a.Id == id);
            if (uri != null)
            {
                _configurationDbContext.ClientRedirectUris.Remove(uri);
                await _configurationDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit), "Client", new { id = clientId });
        }

        [HttpPost("{clientId}/addRedirectUri")]
        public async Task<IActionResult> AddRedirectUri(string clientId, [FromForm] AddRedirectUriModel model)
        {
            var uriCount = _configurationDbContext.ClientRedirectUris.Where(a => a.Client.ClientId == clientId && a.RedirectUri == model.RedirectUri).Count();
            if (uriCount == 0)
            {
                await _configurationDbContext.ClientRedirectUris.AddAsync(new ClientRedirectUri
                {
                    Client = _configurationDbContext.Clients.First(a => a.ClientId == clientId),
                    RedirectUri = model.RedirectUri,
                });
                await _configurationDbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Edit), "Client", new { id = clientId });
        }
    }
}