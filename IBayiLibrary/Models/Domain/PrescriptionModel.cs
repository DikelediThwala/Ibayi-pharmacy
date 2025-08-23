using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Models.Domain
{
    public class PrescriptionModel
    {         
            public int UnprocessedPrescriptionID { get; set; }       
            public int PrescriptionID { get; set; }
            public int PharmacistID { get; set; }
            public string FirstName { get; set; }
            public string Instructions { get; set; }
            public DateTime Date { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
            public int Repeats { get; set; }
            public int RepeatsLeft { get; set; }
            public int Quantity { get; set; }
            public string MedicineName { get; set; }
            public byte[] PrescriptionPhoto { get; set; }
        

    }
}
