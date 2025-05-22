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

            public int PharmacyManagerID { get; set; }

            [ForeignKey("PharmacyManagerID")]
            public Supplier PharmacyManager { get; set; }

            public int SupplierID { get; set; }

            [ForeignKey("SupplierID")]
            public Supplier Supplier { get; set; }

            public List<B_OrderLine> OrderLines { get; set; }
      
    }
}
