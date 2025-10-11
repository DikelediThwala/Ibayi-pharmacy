using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace ONT_PROJECT.Models;

public partial class BOrderLine
{
    public int BOrderLineId { get; set; }
    public int BOrderId { get; set; }

    public int MedicineId { get; set; }
    [NotMapped] 
    public bool IsSelected { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; } = "Pending";

    public virtual BOrder? BOrder { get; set; }
    public virtual Medicine? Medicine { get; set; }
}
