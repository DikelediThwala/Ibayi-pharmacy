using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONT_PROJECT.Models;
public partial class DosageForm
{
    public int FormId { get; set; }

    [Required(ErrorMessage = "The Form Name field is required.")]
    public string FormName { get; set; } = null!;

    public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
}

