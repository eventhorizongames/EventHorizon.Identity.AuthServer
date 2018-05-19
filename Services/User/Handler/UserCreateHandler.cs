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
    public class UserCreateHandler : IRequestHandler<UserCreateEvent, IdentityResult>
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserCreateHandler(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        public async Task<IdentityResult> Handle(UserCreateEvent notification, CancellationToken cancellationToken)
        {
            var result = await _userManager.CreateAsync(notification.User, notification.Password);
            if (result.Succeeded)
            {
                await _mediator.Publish(new UserSetProfileClaimsEvent
                {
                    User = notification.User,
                    Profile = notification.Profile,
                });
            }
            return result;
        }
    }
}