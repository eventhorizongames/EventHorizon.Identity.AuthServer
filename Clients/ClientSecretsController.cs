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
        [HttpPost("{clientId}/removeSecret/{id:int}")]
        public async Task<IActionResult> RemoveSecret(string clientId, int id)
        {
            var secret = _configurationDbContext.ClientSecrets
                .FirstOrDefault(a => a.Client.ClientId == clientId && a.Id == id);
            if (secret != null)
            {
                _configurationDbContext.ClientSecrets.Remove(secret);
                await _configurationDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit), "Client", new { id = clientId });
        }

        [HttpPost("{clientId}/addSecret")]
        public async Task<IActionResult> AddSecret(string clientId, [FromForm] AddSecretModel model)
        {
            var secretCount = _configurationDbContext.ClientSecrets.Where(a => a.Client.ClientId == clientId && a.Value == model.Value.Sha256()).Count();
            if (secretCount == 0)
            {
                await _configurationDbContext.ClientSecrets.AddAsync(new ClientSecret
                {
                    Client = _configurationDbContext.Clients.First(a => a.ClientId == clientId),
                    Description = model.Description,
                    Value = model.Value.Sha256(),
                });
                await _configurationDbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Edit), "Client", new { id = clientId });
        }
    }
}