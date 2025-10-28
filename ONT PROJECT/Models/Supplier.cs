using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ONT_PROJECT.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Contact name is required.")]
    [StringLength(50, ErrorMessage = "Contact name cannot exceed 50 characters.")]
    public string ContactName { get; set; } = null!;

    [Required(ErrorMessage = "Contact surname is required.")]
    [StringLength(50, ErrorMessage = "Contact surname cannot exceed 50 characters.")]
    public string ContactSurname { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email with '@' and domain.")] 
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Status is required")]
    [RegularExpression(@"^(Active|Inactive)$", ErrorMessage = "Status must be either Active or Inactive")]
    public string Status { get; set; } = "Active";

    public virtual ICollection<BOrder> BOrders { get; set; } = new List<BOrder>();
}
