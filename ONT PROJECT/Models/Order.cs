using System;
using System.Collections.Generic;

namespace ONT_PROJECT.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public int PharmacistId { get; set; }

    public string Status { get; set; } = null!;

    public double TotalDue { get; set; }

    public double Vat { get; set; }

    public int SupplierId { get; set; }

    public DateOnly DatePlaced { get; set; }

    public DateOnly? DateRecieved { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

    public virtual Pharmacist Pharmacist { get; set; } = null!;
}
