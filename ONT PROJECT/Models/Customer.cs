using System;
using System.Collections.Generic;

namespace ONT_PROJECT.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public virtual ICollection<CustomerAllergy> CustomerAllergies { get; set; } = new List<CustomerAllergy>();

    public virtual TblUser CustomerNavigation { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    public virtual ICollection<UnprocessedPrescription> UnprocessedPrescriptions { get; set; } = new List<UnprocessedPrescription>();
}
