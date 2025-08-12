using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Models.Domain
{
    public class PrescriptionModel
    {
        
            public int PrescriptionID { get; set; }
            public string FirstName { get; set; }
            public DateTime Date { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
            public int Repeats { get; set; }
            public int RepeatsLeft { get; set; }
            public string MedicineName { get; set; }
            public byte[] PrescriptionPhoto { get; set; }
        

    }
}
