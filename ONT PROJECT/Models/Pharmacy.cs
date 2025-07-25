using System;
using System.Collections.Generic;

namespace ONT_PROJECT.Models;

public partial class Pharmacy
{
    public int PharmacyId { get; set; }

    public string Name { get; set; } = null!;

    public string HealthCounsilRegistrationNo { get; set; } = null!;

    public string ContactNo { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string WebsiteUrl { get; set; } = null!;

    public int? PharmacistId { get; set; }

    public byte[]? Logo { get; set; }

    public string Address { get; set; } = null!;

    public double Vatrate { get; set; }

    public virtual Pharmacist? Pharmacist { get; set; }
}
