using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ONT_PROJECT.Models;

public partial class PrescriptionLine
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PrescriptionLineId { get; set; }


    //[Required(ErrorMessage = "Prescription is required.")]
    public int PrescriptionId { get; set; }



    [Required(ErrorMessage = "Medicine is required.")]
    public int MedicineId { get; set; }

    public string Instructions { get; set; } = null!;
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public int? RepeatsLeft { get; set; }
    public int? Repeats { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    public virtual Medicine Medicine { get; set; } = null!;

    public virtual Prescription Prescription { get; set; } = null!;
}
