using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ONT_PROJECT.Models;

public partial class Doctor
{
    public int DoctorId { get; set; }

    [Required(ErrorMessage = "First Name is required.")]
    [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Surname is required.")]
    [StringLength(50, ErrorMessage = "Surname cannot exceed 50 characters.")]
    public string Surname { get; set; } = null!;

    [Required(ErrorMessage = "Practice Number is required.")]
    [StringLength(20, ErrorMessage = "Practice Number cannot exceed 20 characters.")]
    public string PracticeNo { get; set; } = null!;

    [Required(ErrorMessage = "Contact Number is required.")]
    [RegularExpression(@"^(?:0\d{9}|\+27\d{9})$",
     ErrorMessage = "Contact Number must be a valid South African number.")]
    public string ContactNo { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email with '@' and domain.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string Email { get; set; } = null!;

    public string Status { get; set; } = "Active";

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
