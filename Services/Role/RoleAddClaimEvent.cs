using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EventHorizon.Identity.AuthServer.Services.Role
{
    public class RoleAddClaimEvent : IRequest<IdentityResult>
    {
        public string RoleName { get; set; }
        public Claim Claim { get; set; }
    }
}