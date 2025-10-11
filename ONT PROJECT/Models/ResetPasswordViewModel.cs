using System.ComponentModel.DataAnnotations;

namespace ONT_PROJECT.Models
{
    public class ResetPasswordViewModel
    {
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Enter Temporary Password")]
        public string TemporaryPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
