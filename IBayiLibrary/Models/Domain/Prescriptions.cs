using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IBayiLibrary.Models.Domain
{
    public class Prescriptions
    {

        public int PrescriptionID { get; set; }
        public DateTime Date { get; set; }



        [ForeignKey("CustomerID")]
        public int CustomerID { get; set; }

        [ForeignKey("PharmacistID")]
        public int PharmacistID { get; set; }

        public byte[] PrescriptionPhoto { get; set; }
        public string Status { get; set; }
        [NotMapped]
        public IFormFile? PescriptionFile { get; set; }
        [ForeignKey("DoctorID")]
        public int DoctorID { get; set; }
       
    }
}
