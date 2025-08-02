using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ONT_PROJECT.Models;

public partial class Medicine
{
    public int MedicineId { get; set; }

    public string MedicineName { get; set; } = null!;

    public int Schedule { get; set; }

    [NotMapped]
    public List<int> Ingredients { get; set; } = new List<int>();

    public double SalesPrice { get; set; }

    public int SupplierId { get; set; }

    public int ReorderLevel { get; set; }

    public int Quantity { get; set; }

    [Required(ErrorMessage = "Please select a Form.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Form.")]
    public int? FormId { get; set; }


    public virtual ICollection<BOrderLine> BOrderLines { get; set; } = new List<BOrderLine>();

    public virtual DosageForm Form { get; set; } = null!;

    public virtual ICollection<MedIngredient> MedIngredients { get; set; } = new List<MedIngredient>();

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

    public virtual ICollection<PrescriptionLine> PrescriptionLines { get; set; } = new List<PrescriptionLine>();
}
