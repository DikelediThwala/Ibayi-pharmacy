using System;
using System.Collections.Generic;

namespace ONT_PROJECT.Models;

public partial class MedIngredient
{
    public int MedIngredientId { get; set; }

    public int MedicineId { get; set; }

    public int ActiveIngredientId { get; set; }

    public string Strength { get; set; } = null!;

    public virtual ActiveIngredient ActiveIngredient { get; set; } = null!;

    public virtual Medicine Medicine { get; set; } = null!;
}
