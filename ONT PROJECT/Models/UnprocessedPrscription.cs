using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;

namespace ONT_PROJECT.Models
{
    public class UnprocessedPrscription
    {
        public static string Unprocessed { get; private set; }
        [Key]
        public int UnprocessedPrescriptionID { get; set; }

        [Required]
         public int CustomerID { get; set; }


        public DateOnly Date {  get; set; }

        [Required]
        public byte[] PrescriptionPhoto { get; set; }

        [Required]
        public int Dispense {  get; set; }

        [Required]
        [RegularExpression("Unprocessed|Processed", ErrorMessage = "Status must be either 'Unprocessed' or 'Processed'.")]
        public string Status { get; set; } = UnprocessedPrscription.Unprocessed;
    }
}
