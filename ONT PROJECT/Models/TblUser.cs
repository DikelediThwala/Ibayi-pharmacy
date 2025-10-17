using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ONT_PROJECT.Validators;


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
    [Remote(action: "IsEmailAvailable", controller: "CustomerRegister", ErrorMessage = "An email already exists")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = null!;


    [Required]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "ID Number is required")]
    [Display(Name = "ID Number")]
    [SaIdNumber(ErrorMessage = "Invalid South African ID Number")]
    [Remote(action: "ValidateIdNumber", controller: "User", ErrorMessage = "Invalid ID Number")]
    public string Idnumber { get; set; } = null!;



    [Required(ErrorMessage = "Role is required")]
    public string Role { get; set; } = null!;

    [Required(ErrorMessage = "Phone Number is required")]
    [Display(Name = "Phone Number")]
    [RegularExpression(@"^(?:0|\+27)\d{9}$", ErrorMessage = "Invalid contact number")]
    public string PhoneNumber { get; set; } = null!;


    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = null!;
    public string Status { get; set; } = "Active";

    public string? Allergies { get; set; } = null!;
    [NotMapped]
    public List<int> SelectedAllergyIds { get; set; } = new();
    public string? ResetToken { get; set; }
    public DateTime? TokenExpiry { get; set; }

    public byte[]? ProfilePicture { get; set; }

    [NotMapped]
    public IFormFile? ProfileFile { get; set; }
    public virtual Customer? Customer { get; set; }

    public virtual Pharmacist? Pharmacist { get; set; }

    public virtual PharmacyManager? PharmacyManager { get; set; }
}

