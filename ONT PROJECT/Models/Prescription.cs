using ONT_PROJECT.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONT_PROJECT.Models
{
    public class Prescription
    {
        [Key]
        public int PrescriptionID { get; set; }
        public DateOnly Date {  get; set; }

      
        

        [ForeignKey("CustomerID")]
        public Customer CustomerID { get; set; }

        [ForeignKey("PharmacistID")]
        public Pharmacist PharmacistID { get; set; }

        public byte[] PrescriptionPhoto { get; set; }
        public string Status { get; internal set; }

        [ForeignKey("DoctorID")]
        public Doctor DoctorID { get; set; }
    }
}
