using EventHorizon.Identity.AuthServer.Models;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Services.User
{
    public struct PurgeUserAccount
        : IRequest<bool>
    {
        public ApplicationUser User { get; }

        public PurgeUserAccount(
            ApplicationUser user
        )
        {
            User = user;
        }
    }
}
