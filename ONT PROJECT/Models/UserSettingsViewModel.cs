namespace ONT_PROJECT.Models
{
    public class UserSettingsViewModel
    {
        // Personal Details
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public byte[]? ExistingProfilePicture { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

}
