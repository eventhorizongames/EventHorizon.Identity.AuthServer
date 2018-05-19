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
        [HttpPost("{clientId}/removeScope/{id:int}")]
        public async Task<IActionResult> RemoveScope(string clientId, int id)
        {
            var scope = _configurationDbContext.ClientScopes
                .FirstOrDefault(a => a.Client.ClientId == clientId && a.Id == id);
            if (scope != null)
            {
                _configurationDbContext.ClientScopes.Remove(scope);
                await _configurationDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit), "Client", new { id = clientId });
        }

        [HttpPost("{clientId}/addScope")]
        public async Task<IActionResult> AddScope(string clientId, [FromForm] AddScopeModel model)
        {
            var scopeCount = _configurationDbContext.ClientScopes.Where(a => a.Client.ClientId == clientId && a.Scope == model.Scope).Count();
            if (scopeCount == 0)
            {
                await _configurationDbContext.ClientScopes.AddAsync(new ClientScope
                {
                    Client = _configurationDbContext.Clients.First(a => a.ClientId == clientId),
                    Scope = model.Scope,
                });
                await _configurationDbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Edit), "Client", new { id = clientId });
        }
    }
}