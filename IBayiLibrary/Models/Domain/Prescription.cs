using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Models.Domain
{
    public class Prescription
    {
      
        public int PrescriptionID { get; set; }
        public DateOnly Date { get; set; }

       

        //[ForeignKey("CustomerID")]
        public int CustomerID { get; set; }

        //[ForeignKey("PharmacistID")]
        public int PharmacistID { get; set; }

        public byte[] PrescriptionPhoto { get; set; }
        //public string Status { get; internal set; }

        //[ForeignKey("DoctorID")]
        public int DoctorID { get; set; }
    }
}
