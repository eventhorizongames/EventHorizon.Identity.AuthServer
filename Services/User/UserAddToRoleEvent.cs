using System.Collections.Generic;
using System.Security.Claims;
using EventHorizon.Identity.AuthServer.Models;
using MediatR;

namespace EventHorizon.Identity.AuthServer.Services.User
{
    public struct UserAddToRoleEvent : INotification
    {
        public ApplicationUser User { get; internal set; }
        public string Role { get; internal set; }
    }
}