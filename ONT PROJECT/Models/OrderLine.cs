using System;
using System.Collections.Generic;

namespace ONT_PROJECT.Models;

public partial class OrderLine
{
    public int OrderLineId { get; set; }

    public int OrderId { get; set; }

    public int MedicineId { get; set; }

    public int Quantity { get; set; }

    public int LineId { get; set; }

    public double Price { get; set; }

    public double LineTotal { get; set; }

    public string Status { get; set; } = null!;

    public virtual Medicine Medicine { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
