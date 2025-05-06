using System.ComponentModel.DataAnnotations;

namespace ONT_PROJECT.Models
{
    public class User
    {
        public int UserID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string IDNumber { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        public byte[] ProfilePicture { get; set; }
    }

}
