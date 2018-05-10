using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EventHorizon.Identity.AuthServer.Models;

namespace EventHorizon.Identity.AuthServer.Manage.Models
{
    public class IndexViewModel
    {
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public ProfileModel Profile { get; set; } = new ProfileModel();

        public string StatusMessage { get; set; }
    }
}