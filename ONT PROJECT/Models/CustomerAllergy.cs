using System;
using System.Collections.Generic;

namespace ONT_PROJECT.Models;

public partial class CustomerAllergy
{
    public int CustomerAllergyId { get; set; }

    public int CustomeId { get; set; }

    public int ActiveIngredientsId { get; set; }

    public virtual ActiveIngredient ActiveIngredients { get; set; } = null!;

    public virtual Customer Custome { get; set; } = null!;
}
