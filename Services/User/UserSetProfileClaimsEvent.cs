using EventHorizon.Identity.AuthServer.Models;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Services.User
{
    public struct UserSetProfileClaimsEvent : INotification
    {
        public ApplicationUser User { get; }
        public ApplicationUserProfile Profile { get; }
        public UserSetProfileClaimsEvent(
            ApplicationUser user,
            ApplicationUserProfile profile
        )
        {
            this.User = user;
            this.Profile = profile;
        }
    }
}