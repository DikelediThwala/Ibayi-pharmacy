using System;
using System.Collections.Generic;

namespace ONT_PROJECT.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string Name { get; set; } = null!;

    public string ContactNo { get; set; } = null!;

    public string Email { get; set; } = null!;
    public string Status { get; set; } = "Active";


    public virtual ICollection<BOrder> BOrders { get; set; } = new List<BOrder>();
}
