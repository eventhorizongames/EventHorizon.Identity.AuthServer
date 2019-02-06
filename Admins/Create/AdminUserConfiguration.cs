using System.Collections.Generic;

namespace EventHorizon.Identity.AuthServer.Admins.Create
{
    public class AdminUserConfiguration
    {
        public List<AdminUser> Admins { get; set; }
    }
    public class AdminUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}