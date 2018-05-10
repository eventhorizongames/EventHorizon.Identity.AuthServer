using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Manage.Models;
using EventHorizon.Identity.AuthServer.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace EventHorizon.Identity.AuthServer.Services.User
{
    public class UserCreation : IUserCreation
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserCreation(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> Create(ApplicationUser user, ApplicationUserProfile profile, string password)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Create Claims
                await this.ReplaceClaims(user, profile);
            }

            return result;
        }

        public async Task ReplaceClaims(ApplicationUser user, ApplicationUserProfile profile)
        {
            var claimList = await _userManager.GetClaimsAsync(user);
            await ReplaceClaim(claimList, user, JwtClaimTypes.Name, profile.FirstName);
            await ReplaceClaim(claimList, user, JwtClaimTypes.GivenName, profile.FirstName);
            await ReplaceClaim(claimList, user, JwtClaimTypes.FamilyName, profile.LastName);
            await ReplaceClaim(claimList, user, JwtClaimTypes.Email, user.Email);
            await ReplaceClaim(claimList, user, JwtClaimTypes.PhoneNumber, profile.PhoneNumber);
        }

        private async Task ReplaceClaim(IList<Claim> claimList, ApplicationUser user, string claimType, string claimValue, string defaultValue = "")
        {
            var userClaim = claimList.FirstOrDefault(a => a.Type == claimType);
            if (userClaim != null)
            {
                if (userClaim.Value != claimValue)
                {
                    await _userManager.ReplaceClaimAsync(user, userClaim, new Claim(claimType, claimValue ?? defaultValue));
                }
            }
            else
            {
                await _userManager.AddClaimAsync(user, new Claim(claimType, claimValue ?? defaultValue));
            }
        }
    }
}