using EventHorizon.Identity.AuthServer.Models;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Services.User
{
    public struct UserSetProfileClaimsEvent : INotification
    {
        public ApplicationUser User { get; internal set; }
        public ApplicationUserProfile Profile { get; internal set; }
    }
}