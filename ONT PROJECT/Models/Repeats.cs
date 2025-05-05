using System.ComponentModel.DataAnnotations;

namespace ONT_PROJECT.Models
{
    public class Repeats
    {
        [Required]
        public int CustomerID { get; set; }

        [Required]
        public int repeatsLeft { get; set; }
    }
}
