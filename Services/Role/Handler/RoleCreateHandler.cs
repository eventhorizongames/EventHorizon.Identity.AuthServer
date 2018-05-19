using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EventHorizon.Identity.AuthServer.Services.Role.Handler
{
    public class RoleCreateHandler : IRequestHandler<RoleCreateEvent, IdentityResult>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleCreateHandler(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<IdentityResult> Handle(RoleCreateEvent notification, CancellationToken cancellationToken)
        {
            // Create Role
            return await _roleManager.CreateAsync(new IdentityRole(notification.RoleName));
        }
    }
}