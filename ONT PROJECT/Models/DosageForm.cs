using System;
using System.Collections.Generic;

namespace ONT_PROJECT.Models;

public partial class DosageForm
{
    public int FormId { get; set; }

    public string Form { get; set; } = null!;

    public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
}
