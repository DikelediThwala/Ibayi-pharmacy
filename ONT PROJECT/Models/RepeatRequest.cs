using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONT_PROJECT.Models
{
    public class RepeatRequest
    {
        [Key]
        public int RepeatRequestId { get; set; }

        // Use OrderLineId as the foreign key
        [Required]
        public int OrderLineId { get; set; }

        [ForeignKey("OrderLineId")]
        public virtual OrderLine OrderLine { get; set; }

        [Required]
        public DateTime RequestDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Processed
    }
}
