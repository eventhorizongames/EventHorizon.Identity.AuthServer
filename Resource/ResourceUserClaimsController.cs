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
        [HttpPost("{resourceId}/removeUserClaim/{id:int}")]
        public async Task<IActionResult> RemoveUserClaim(string resourceId, int id)
        {
            var apiResource = _configurationDbContext.ApiResources
                .Include("UserClaims")
                .FirstOrDefault(a => a.Name == resourceId);
            var userClaim = apiResource.UserClaims.FirstOrDefault(a => a.Id == id);
            if (userClaim != null)
            {
                apiResource.UserClaims.Remove(userClaim);
                _configurationDbContext.Update(apiResource);
                await _configurationDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit), "Resource", new { id = resourceId });
        }

        [HttpPost("{resourceId}/addUserClaim")]
        public async Task<IActionResult> AddUserClaim([FromRoute] string resourceId, [FromForm] string type)
        {
            var apiResource = _configurationDbContext.ApiResources
                .Include("UserClaims")
                .FirstOrDefault(a => a.Name == resourceId);
            if (apiResource.UserClaims.FirstOrDefault(a => a.Type == type) == null)
            {
                apiResource.UserClaims.Add(new ApiResourceClaim
                {
                    Type = type,
                });
                _configurationDbContext.Update(apiResource);
                await _configurationDbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Edit), "Resource", new { id = resourceId });
        }
    }
}