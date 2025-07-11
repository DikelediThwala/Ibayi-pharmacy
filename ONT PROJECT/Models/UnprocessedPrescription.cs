using System;
using System.Collections.Generic;

namespace ONT_PROJECT.Models;

public partial class UnprocessedPrescription
{
    public int UnprocessedPrescriptionId { get; set; }

    public int CustomerId { get; set; }

    public DateOnly Date { get; set; }

    public byte[] PrescriptionPhoto { get; set; } = null!;

    public string Dispense { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
}
