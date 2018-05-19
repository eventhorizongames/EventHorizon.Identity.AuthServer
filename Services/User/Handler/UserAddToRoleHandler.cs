using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Models;
using EventHorizon.Identity.AuthServer.Services.Role;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EventHorizon.Identity.AuthServer.Services.User.Handler
{
    public class UserAddToRoleHandler : INotificationHandler<UserAddToRoleEvent>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserAddToRoleHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task Handle(UserAddToRoleEvent notification, CancellationToken cancellationToken)
        {
            await _userManager.AddToRoleAsync(notification.User, notification.Role);
        }
    }
}