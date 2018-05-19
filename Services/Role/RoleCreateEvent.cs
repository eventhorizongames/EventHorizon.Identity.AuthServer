using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EventHorizon.Identity.AuthServer.Services.Role
{
    public class RoleCreateEvent : IRequest<IdentityResult>
    {
        public string RoleName { get; set; }
    }
}