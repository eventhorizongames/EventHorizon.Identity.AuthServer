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
        [HttpPost("{clientId}/removeCorsOrigin/{id:int}")]
        public async Task<IActionResult> RemoveCorsOrigin(string clientId, int id)
        {
            var corsOrigin = _configurationDbContext.ClientCorsOrigins
                .FirstOrDefault(a => a.Client.ClientId == clientId && a.Id == id);
            if (corsOrigin != null)
            {
                _configurationDbContext.ClientCorsOrigins.Remove(corsOrigin);
                await _configurationDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit), "Client", new { id = clientId });
        }

        [HttpPost("{clientId}/addCorsOrigin")]
        public async Task<IActionResult> AddCorsOrigin(string clientId, [FromForm] AddAllowedCorsOrign model)
        {
            var corsOriginCount = _configurationDbContext.ClientCorsOrigins.Where(a => a.Client.ClientId == clientId && a.Origin == model.CorsOrigin).Count();
            if (corsOriginCount == 0)
            {
                await _configurationDbContext.ClientCorsOrigins.AddAsync(new ClientCorsOrigin
                {
                    Client = _configurationDbContext.Clients.First(a => a.ClientId == clientId),
                    Origin = model.CorsOrigin,
                });
                await _configurationDbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Edit), "Client", new { id = clientId });
        }
    }
}