using System.Collections.Generic;
using System.Security.Claims;
using EventHorizon.Identity.AuthServer.Models;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Services.User
{
    public struct UserSetClaimEvent : INotification
    {
        public ApplicationUser User { get; set; }
        public IList<Claim> ClaimList { get; internal set; }
        public Claim Claim { get; set; }
    }
}