using System.ComponentModel.DataAnnotations;
using EventHorizon.Identity.AuthServer.Models;

namespace EventHorizon.Identity.AuthServer.Register.Models
{
    public class UserRegistrationModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string CaptchaToken { get; set; }

        public ProfileModel Profile { get; set; } = new ProfileModel();
    }
}