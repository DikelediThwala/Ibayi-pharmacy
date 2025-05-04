using System.ComponentModel.DataAnnotations;

namespace ONT_PROJECT.Models
{
    public class Doctor
    {
        public int DoctorID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string PracticeNo { get; set; }

        [Required]
        [Phone]
        public string ContactNo { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
     
    }
}
