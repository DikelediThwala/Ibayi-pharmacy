using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONT_PROJECT.Models;

public partial class TblUser
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "ID Number is required")]
    [Display(Name = "ID Number")]
    public string Idnumber { get; set; } = null!;

    [Required(ErrorMessage = "Role is required")]
    public string Role { get; set; } = null!;

    [Required(ErrorMessage = "Phone Number is required")]
    [Display(Name = "Phone Number")] // matches the DB column name
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = null!;
    public string Allergies { get; set; } = null!;
    public byte[]? ProfilePicture { get; set; }

    // 🔥 Add this
    [NotMapped]
    public IFormFile? ProfileFile { get; set; }
    public virtual Customer? Customer { get; set; }

    public virtual Pharmacist? Pharmacist { get; set; }

    public virtual PharmacyManager? PharmacyManager { get; set; }
}

























//using System;
//using System.Collections.Generic;

//namespace ONT_PROJECT.Models;

//public partial class TblUser
//{
//    public int UserId { get; set; }

//    public string FirstName { get; set; } = null!;

//    public string LastName { get; set; } = null!;

//    public string Email { get; set; } = null!;

//    public string Password { get; set; } = null!;

//    public string Idnumber { get; set; } = null!;

//    public string Role { get; set; } = null!;

//    public string PhoneNumber { get; set; } = null!;

//    public string Title { get; set; } = null!;

//    public byte[]? ProfilePicture { get; set; }

//    public virtual Customer? Customer { get; set; }

//    public virtual Pharmacist? Pharmacist { get; set; }

//    public virtual PharmacyManager? PharmacyManager { get; set; }
//}
