using System;
using System.Collections.Generic;

namespace ONT_PROJECT.Models;

public partial class PrescriptionLine
{
    public int PrescriptionLineId { get; set; }

    public int PrescriptionId { get; set; }

    public int MedicineId { get; set; }

    public int Quantity { get; set; }

    public string Instructions { get; set; } = null!;

    public int Repeats { get; set; }

    public int RepeatsLeft { get; set; }

    public virtual Medicine Medicine { get; set; } = null!;

    public virtual Prescription Prescription { get; set; } = null!;
}
