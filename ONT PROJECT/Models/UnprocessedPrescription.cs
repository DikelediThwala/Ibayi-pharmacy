using System;
using System.Collections.Generic;

namespace ONT_PROJECT.Models;

public partial class UnprocessedPrescription
{
    public int UnprocessedPrescriptionId { get; set; }

    public int CustomerId { get; set; }

    public DateOnly Date { get; set; }

    public byte[]? PrescriptionPhoto { get; set; }    // nullable
    public string? Dispense { get; set; }             // nullable
    public string? Status { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
