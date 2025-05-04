using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONT_PROJECT.Models
{
    public class B_Order
    {
        [Key]
        public int B_OrderID { get; set; }

        [Required]
        public DateOnly DatePlaced { get; set; }

        [Required]
        public DateOnly DateRecieved { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        [ForeignKey("PharmacyManagerID")]
        public Supplier PharmacyManagerID { get; set; }

        [ForeignKey("SupplierID")]
        public Supplier SupplierID { get; set; }
    }
}
