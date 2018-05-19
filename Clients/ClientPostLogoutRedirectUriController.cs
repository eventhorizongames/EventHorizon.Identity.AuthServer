using System.Linq;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Clients.Models;
using EventHorizon.Identity.AuthServer.Configuration;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Identity.AuthServer.Clients
{
    public partial class ClientController : Controller
    {
        [HttpPost("{clientId}/removePostLogoutRedirectUri/{id:int}")]
        public async Task<IActionResult> RemovePostLogoutRedirectUri(string clientId, int id)
        {
            var uri = _configurationDbContext.ClientPostLogoutRedirectUris
                .FirstOrDefault(a => a.Client.ClientId == clientId && a.Id == id);
            if (uri != null)
            {
                _configurationDbContext.ClientPostLogoutRedirectUris.Remove(uri);
                await _configurationDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit), "Client", new { id = clientId });
        }

        [HttpPost("{clientId}/addPostLogoutRedirectUri")]
        public async Task<IActionResult> AddPostLogoutRedirectUri(string clientId, [FromForm] AddPostLogoutRedirectUriModel model)
        {
            var uriCount = _configurationDbContext.ClientPostLogoutRedirectUris.Where(a => a.Client.ClientId == clientId && a.PostLogoutRedirectUri == model.PostLogoutRedirectUri).Count();
            if (uriCount == 0)
            {
                await _configurationDbContext.ClientPostLogoutRedirectUris.AddAsync(new ClientPostLogoutRedirectUri
                {
                    Client = _configurationDbContext.Clients.First(a => a.ClientId == clientId),
                    PostLogoutRedirectUri = model.PostLogoutRedirectUri,
                });
                await _configurationDbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Edit), "Client", new { id = clientId });
        }
    }
}