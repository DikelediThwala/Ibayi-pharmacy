using System;
using System.Collections.Generic;

namespace ONT_PROJECT.Models;

public partial class ActiveIngredient
{
    public int ActiveIngredientId { get; set; }

    public string Ingredients { get; set; } = null!;

    public virtual ICollection<CustomerAllergy> CustomerAllergies { get; set; } = new List<CustomerAllergy>();

    public virtual ICollection<MedIngredient> MedIngredients { get; set; } = new List<MedIngredient>();
}
