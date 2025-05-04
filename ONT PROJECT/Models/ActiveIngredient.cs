using System.ComponentModel.DataAnnotations;

namespace ONT_PROJECT.Models
{
    public class ActiveIngredient
    {
        public int ActiveIngredientsID { get; set; }

        [Required]
        public string Ingredients { get; set; }
    }
}
