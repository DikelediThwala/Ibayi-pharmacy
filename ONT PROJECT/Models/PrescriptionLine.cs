using System.ComponentModel.DataAnnotations;

namespace ONT_PROJECT.Models
{
    public class PrescriptionLine
    {
        [Key]
        public int LineID { get; set; }

        [Required]
        public int PrescriptionID { get; set; }
         
        [Required]
        public int MedicineID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string Instructions { get; set; }


        public int Repeats { get; set; }

        public int RepeatsLeft { get; set; }
    }
}
