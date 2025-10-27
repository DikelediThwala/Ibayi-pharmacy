using IBayiLibrary.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IBayiLibrary.Models.Domain
{
    public class tblUser
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        public string Password { get; set; }
        [Required(ErrorMessage = "ID Number is required.")]
        [SouthAfricanID]
        public string IDNumber { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public string Title { get; set; }
    }
}
