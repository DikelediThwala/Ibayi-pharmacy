using System;
using System.Collections.Generic;

namespace ONT_PROJECT.Models;

public partial class Prescription
{
    public int PrescriptionId { get; set; }

    public DateOnly Date { get; set; }

    public int CustomerId { get; set; }

    public int PharmacistId { get; set; }

    //public byte[] PrescriptionPhoto { get; set; } = null!;
    public byte[]? PrescriptionPhoto { get; set; }


    public int DoctorId { get; set; }

    public string Status { get; set; } = "Uploaded"; // "Uploaded", "Requested", "Ready"

    public virtual Customer Customer { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Pharmacist Pharmacist { get; set; } = null!;

    public virtual ICollection<PrescriptionLine> PrescriptionLines { get; set; } = new List<PrescriptionLine>();
}
