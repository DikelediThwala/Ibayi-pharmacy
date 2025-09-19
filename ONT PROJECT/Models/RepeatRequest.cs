using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONT_PROJECT.Models
{
    public class RepeatRequest
    {
        [Key]
        public int RepeatRequestId { get; set; }

        [Required]
        public int PrescriptionLineId { get; set; }

        [ForeignKey("PrescriptionLineId")]
        public virtual PrescriptionLine PrescriptionLine { get; set; }

        [Required]
        public DateTime RequestDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Processed
    }
}
