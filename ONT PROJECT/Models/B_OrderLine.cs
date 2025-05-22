using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONT_PROJECT.Models
{
    public class B_OrderLine
    {
        [Key]
        public int B_OrderLineID { get; set; } 

        [Required]
        public int B_OrderID { get; set; }

        [ForeignKey("B_OrderID")]
        public B_Order B_Order { get; set; }

        [Required]
        public int MedicationID { get; set; }

        [ForeignKey("MedicationID")]
        public Medicine Medication { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
