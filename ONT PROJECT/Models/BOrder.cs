using System;
using System.Collections.Generic;

namespace ONT_PROJECT.Models;

public partial class BOrder
{
    public int BOrderId { get; set; }

    public DateOnly DatePlaced { get; set; }

    public DateOnly? DateRecieved { get; set; } // Nullable now

    public string Status { get; set; } = null!;

    public int? PharmacyManagerId { get; set; } // Nullable

    public int? SupplierId { get; set; } // Nullable

    public List<BOrderLine> BOrderLines { get; set; } = new List<BOrderLine>();

    public virtual PharmacyManager? PharmacyManager { get; set; }

    public virtual Supplier? Supplier { get; set; }
}

