using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EventHorizon.Identity.AuthServer.Services.Role.Handler
{
    public class RoleAddClaimHandler : IRequestHandler<RoleAddClaimEvent, IdentityResult>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleAddClaimHandler(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<IdentityResult> Handle(RoleAddClaimEvent notification, CancellationToken cancellationToken)
        {
            // Add claim to role
            var role = await _roleManager.FindByNameAsync(notification.RoleName);
            var dbClaims = await _roleManager.GetClaimsAsync(role);
            var dbClaim = dbClaims.FirstOrDefault(a => a.ValueType == notification.Claim.ValueType && a.Value == notification.Claim.Value);
            if (dbClaim == null)
            {
                return await _roleManager.AddClaimAsync(role, notification.Claim);
            }
            return IdentityResult.Success;
        }
    }
}