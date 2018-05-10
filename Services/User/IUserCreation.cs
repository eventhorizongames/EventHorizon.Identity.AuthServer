using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Models;
using Microsoft.AspNetCore.Identity;

namespace EventHorizon.Identity.AuthServer.Services.User
{
    public interface IUserCreation
    {
         Task<IdentityResult> Create(ApplicationUser user, ApplicationUserProfile profile, string password);
         Task ReplaceClaims(ApplicationUser user, ApplicationUserProfile profile);
    }
}