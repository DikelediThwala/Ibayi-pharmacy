using System.ComponentModel.DataAnnotations;

namespace ONT_PROJECT.Models
{
    public class Allergy
    {
        [Key]
        public int AllergyID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        public int ActiveIngredientsID { get; set; }
    }
}
