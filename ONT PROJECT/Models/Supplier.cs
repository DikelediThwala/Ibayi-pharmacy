using System.ComponentModel.DataAnnotations;

namespace ONT_PROJECT.Models
{
    public class Supplier
    {
        public int SupplierID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(11)]
        public string ContactNo { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(50)]
        public string Email { get; set; }
    }
}

