using System.ComponentModel.DataAnnotations;

namespace ONT_PROJECT.Models
{
    public class UserSettingsViewModel
    {
        // Personal Details
        public int UserId { get; set; } // add this

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public IFormFile? ProfilePicture { get; set; } // nullable, optional upload
        public byte[] ExistingProfilePicture { get; set; } // for displaying existing image
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$",
            ErrorMessage = "Password must be at least 8 characters with uppercase, lowercase, number and special character.")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }

}
