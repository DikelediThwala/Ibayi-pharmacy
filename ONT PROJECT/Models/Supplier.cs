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

    [Required(ErrorMessage = "Contact Number is required.")]
    [RegularExpression(@"^(?:0\d{9}|\+27\d{9})$",
     ErrorMessage = "Contact Number must be a valid South African number.")]
    public string ContactNo { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email with '@' and domain.")] 
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Status is required")]
    [RegularExpression(@"^(Active|Inactive)$", ErrorMessage = "Status must be either Active or Inactive")]
    public string Status { get; set; } = "Active";

    public virtual ICollection<BOrder> BOrders { get; set; } = new List<BOrder>();
}
