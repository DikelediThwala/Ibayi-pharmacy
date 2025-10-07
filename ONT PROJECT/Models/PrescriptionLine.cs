using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONT_PROJECT.Models
{
    public partial class PrescriptionLine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PrescriptionLineId { get; set; }

        [Required]
        public int PrescriptionId { get; set; }

        [Required]
        public int MedicineId { get; set; }

        [Required]
        [StringLength(50)]
        public string Instructions { get; set; } = string.Empty;

        [Required]
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public int? RepeatsLeft { get; set; }
        public int? Repeats { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public int? UnprocessedPrescriptionID { get; set; }  // Map the DB column

        public virtual Medicine Medicine { get; set; } = null!;
        public virtual Prescription Prescription { get; set; } = null!;
        public ICollection<RepeatHistory> RepeatHistories { get; set; } = new List<RepeatHistory>();
    }
}
