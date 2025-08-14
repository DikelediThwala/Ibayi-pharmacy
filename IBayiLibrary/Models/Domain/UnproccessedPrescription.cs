using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Models.Domain
{
    public class UnproccessedPrescription
    {
        public int UnproccessedPrescriptionID { get; set; }
        public string FirstName { get; set; }   
        public DateTime Date { get; set; }     
        public byte[] PrescriptionPhoto { get; set; }
        public string Dispened { get; set; }
        public string Status { get; set; }
        [NotMapped]
        public IFormFile? PescriptionFile { get; set; }
        
        


    }
}
