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
        [HttpPost("{clientId}/removeGrant/{id:int}")]
        public async Task<IActionResult> RemoveGrant(string clientId, int id)
        {
            var grantType = _configurationDbContext.ClientGrantTypes
                .FirstOrDefault(a => a.Client.ClientId == clientId && a.Id == id);
            if (grantType != null)
            {
                _configurationDbContext.ClientGrantTypes.Remove(grantType);
                await _configurationDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit), "Client", new { id = clientId });
        }

        [HttpPost("{clientId}/addGrant")]
        public async Task<IActionResult> AddGrant(string clientId, [FromForm] AddGrantModel model)
        {
            var grantTypeCount = _configurationDbContext.ClientGrantTypes.Where(a => a.Client.ClientId == clientId && a.GrantType == model.GrantTypeSelect).Count();
            if (grantTypeCount == 0)
            {
                await _configurationDbContext.ClientGrantTypes.AddAsync(new ClientGrantType
                {
                    Client = _configurationDbContext.Clients.First(a => a.ClientId == clientId),
                    GrantType = model.GrantTypeSelect,
                });
                await _configurationDbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Edit), "Client", new { id = clientId });
        }
    }
}