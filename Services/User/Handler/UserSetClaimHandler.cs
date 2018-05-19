using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EventHorizon.Identity.AuthServer.Services.User.Handler
{
    public class UserSetClaimHandler : INotificationHandler<UserSetClaimEvent>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserSetClaimHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task Handle(UserSetClaimEvent notification, CancellationToken cancellationToken)
        {
            var claim = notification.Claim;
            var user = notification.User;
            var claimList = notification.ClaimList ?? await _userManager.GetClaimsAsync(user);
            var userClaim = claimList.FirstOrDefault(a => a.Type == claim.Type);
            if (userClaim != null)
            {
                if (userClaim.Value != claim.Value)
                {
                    await _userManager.ReplaceClaimAsync(user, userClaim, claim);
                }
            }
            else
            {
                await _userManager.AddClaimAsync(user, claim);
            }
        }
    }
}