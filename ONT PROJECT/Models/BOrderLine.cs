using System;
using System.Collections.Generic;


namespace ONT_PROJECT.Models;

public partial class BOrderLine
{
    public int BOrderLineId { get; set; }
    public int BOrderId { get; set; }

    public int MedicineId { get; set; }

    public int Quantity { get; set; }
    public virtual BOrder? BOrder { get; set; }
    public virtual Medicine? Medicine { get; set; }
}
