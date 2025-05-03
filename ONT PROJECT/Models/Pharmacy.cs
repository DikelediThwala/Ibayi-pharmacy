using ONT_PROJECT.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONT_PROJECT.Models
{
    public class Pharmacy
    {
        [Key]
        public int PharmacyID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Health Council Registration No")]
        public string HealthCounsilRegistrationNo { get; set; }

        [Required]
        [StringLength(11)]
        [Display(Name = "Contact Number")]
        public string ContactNo { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(50)]
        [Url]
        [Display(Name = "Website URL")]
        public string WebsiteUrl { get; set; }

        [Required]
        [ForeignKey("Pharmacist")]
        public int PharmacistID { get; set; }

        public byte[] Logo { get; set; }

        [Required]
        [StringLength(50)]
        public string Address { get; set; }

        [Required]
        [Display(Name = "VAT Rate")]
        public float VATRate { get; set; }

        // Navigation property
        //public Pharmacist Pharmacist { get; set; }
    }
}
