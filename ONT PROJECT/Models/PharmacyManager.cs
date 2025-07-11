using System;
using System.Collections.Generic;

namespace ONT_PROJECT.Models;

public partial class PharmacyManager
{
    public int PharmacyManagerId { get; set; }

    public virtual ICollection<BOrder> BOrders { get; set; } = new List<BOrder>();

    public virtual TblUser PharmacyManagerNavigation { get; set; } = null!;
}
