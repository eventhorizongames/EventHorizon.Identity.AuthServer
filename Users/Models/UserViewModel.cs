using System.Collections.Generic;
using EventHorizon.Identity.AuthServer.Models;
using IdentityServer4.EntityFramework.Entities;

namespace EventHorizon.Identity.AuthServer.Users.Models
{
    public class UserViewModel
    {
        public int Count { get; set; }
        public List<UserDisplayModel> Users { get; } = new List<UserDisplayModel>();
    }
}