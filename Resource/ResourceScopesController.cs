using System.Linq;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Clients.Models;
using EventHorizon.Identity.AuthServer.Configuration;
using EventHorizon.Identity.AuthServer.Resource.Models;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.EntityFramework.Mappers;
using EventHorizon.Identity.AuthServer.Clients;
using IdentityServer4.EntityFramework.Entities;

namespace EventHorizon.Identity.AuthServer.Resource
{
    public partial class ResourceController : Controller
    {
        [HttpPost("{resourceId}/removeScope/{id:int}")]
        public async Task<IActionResult> RemoveScope(string resourceId, int id)
        {
            var apiResource = _configurationDbContext.ApiResources
                .Include("Scopes")
                .FirstOrDefault(a => a.Name == resourceId);
            var scope = apiResource.Scopes.FirstOrDefault(a => a.Id == id);
            if (scope != null)
            {
                apiResource.Scopes.Remove(scope);
                _configurationDbContext.Update(apiResource);
                await _configurationDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit), "Resource", new { id = resourceId });
        }

        [HttpPost("{resourceId}/addScope")]
        public async Task<IActionResult> AddScope([FromRoute] string resourceId, [FromForm] AddResourceScopeModel model)
        {
            var apiResource = _configurationDbContext.ApiResources
                .Include("Scopes")
                .FirstOrDefault(a => a.Name == resourceId);
            if (apiResource.Scopes.FirstOrDefault(a => a.Name == model.Name) == null)
            {
                apiResource.Scopes.Add(new ApiScope
                {
                    Name = model.Name,
                    DisplayName = model.DisplayName
                });
                _configurationDbContext.Update(apiResource);
                await _configurationDbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Edit), "Resource", new { id = resourceId });
        }
    }
}