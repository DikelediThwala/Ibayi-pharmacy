using System.ComponentModel.DataAnnotations;

namespace ONT_PROJECT.Models
{
    public class Pharmacist
    {
        [Key]
        public int PharmacistID { get; set; }

        [Required]
        public string HealthRegNo { get; set; }     
    }
}
