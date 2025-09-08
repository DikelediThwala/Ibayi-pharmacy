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
        public byte[]? ExistingProfilePicture { get; set; } // for displaying existing image
       
    }

}
