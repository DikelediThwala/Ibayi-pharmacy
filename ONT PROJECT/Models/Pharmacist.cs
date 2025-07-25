using System;
using System.Collections.Generic;

namespace ONT_PROJECT.Models;

public partial class Pharmacist
{
    public int PharmacistId { get; set; }

    public string HealthCounsilRegNo { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Pharmacy> Pharmacies { get; set; } = new List<Pharmacy>();

    public virtual TblUser PharmacistNavigation { get; set; } = null!;

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
