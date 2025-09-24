using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Models.Domain
{
    public class PrescriptionViewModel
    {
        public string FirstName {  get; set; }
         public string Name { get; set; }
        public string MedicineName { get; set; }
        public int PrescrptionLineID { get; set; }      
        public int MedicineID { get; set; }
        [ForeignKey("PrescriptionID")]
        public int PrescriptionID { get; set; }
        public int UnprocessedPrescriptionID { get; set; }
        public string Instructions { get; set; }
        public string IDNumber { get; set; }
        public string LastName { get; set; }
        public int Quantity { get; set; }
        public int Repeats { get; set; }
        public int RepeatsLeft { get; set; }
        public DateTime ?Date { get; set; }      
        public int CustomerID { get; set; }   
        public int PharmacistID { get; set; }
        public byte[] PrescriptionPhoto { get; set; }
        public string Status { get; set; }
        [NotMapped]
        public IFormFile? PescriptionFile { get; set; }
        [ForeignKey("DoctorID")]
        public int DoctorID { get; set; }
    }
}
