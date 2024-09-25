using System.ComponentModel.DataAnnotations;

namespace Dealora.Models.ViewModel
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Current Password is required.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New Password is required.")]
        [StringLength(100, ErrorMessage = "New Password must be at least 6 characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("NewPassword", ErrorMessage = "Password and Confirm Password do not match.")]
        public string ConPassword { get; set; }
    }

}
