using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Manage.Models;
using EventHorizon.Identity.AuthServer.Models;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EventHorizon.Identity.AuthServer.Services.User
{
    public class UserSetProfileClaimsHandler : INotificationHandler<UserSetProfileClaimsEvent>
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserSetProfileClaimsHandler(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        public async Task Handle(UserSetProfileClaimsEvent notification, CancellationToken cancellationToken)
        {
            var user = notification.User;
            var profile = notification.Profile;
            var claimList = await _userManager.GetClaimsAsync(user);
            await SetClaim(claimList, user, JwtClaimTypes.Name, profile.FirstName);
            await SetClaim(claimList, user, JwtClaimTypes.GivenName, profile.FirstName);
            await SetClaim(claimList, user, JwtClaimTypes.FamilyName, profile.LastName);
            await SetClaim(claimList, user, JwtClaimTypes.Email, user.Email);
            await SetClaim(claimList, user, JwtClaimTypes.PhoneNumber, profile.PhoneNumber);
        }

        private Task SetClaim(IList<Claim> claimList, ApplicationUser user, string claimType, string claimValue, string defaultValue = "")
        {
            return _mediator.Publish(new UserSetClaimEvent
            {
                User = user,
                ClaimList = claimList,
                Claim = new Claim(claimType, claimValue ?? defaultValue),
            });
        }
    }
}