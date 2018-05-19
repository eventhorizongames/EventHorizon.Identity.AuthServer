using EventHorizon.Identity.AuthServer.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EventHorizon.Identity.AuthServer.Services.User
{
    public struct UserCreateEvent : IRequest<IdentityResult>
    {
        public ApplicationUser User { get; set; }
        public ApplicationUserProfile Profile { get; set; }
        public string Password { get; set; }
    }
}