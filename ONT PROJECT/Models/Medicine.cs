using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ONT_PROJECT.Models
{
    public class Medicine
    {
        [Key]
        public int MedicineID { get; set; }

        [Required]
        [StringLength(50)]
        public string MedicineName { get; set; }

        [Required]
        public int Schedule { get; set; }

        [Required]
        [StringLength(50)]
        public List<string> Ingredients { get; set; } = new List<string>();


        [Required]
        public float SalesPrice { get; set; }

        [Required]
        public int SupplierID { get; set; }

        [Required]
        public int ReorderLevel { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int FormID { get; set; }

        // Navigation property to the Supplier model
        [ForeignKey("SupplierID")]
        public Supplier Supplier { get; set; }
    }

}
