using System;
using System.Collections.Generic;

namespace ONT_PROJECT.Models;

public partial class Doctor
{
    public int DoctorId { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string PracticeNo { get; set; } = null!;

    public string ContactNo { get; set; } = null!;

    public string Email { get; set; } = null!;
    public string Status { get; set; } = "Active";

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
