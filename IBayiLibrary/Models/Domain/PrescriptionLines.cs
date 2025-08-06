using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Models.Domain
{
    public class PrescriptionLines
    {
        public int PrescrptionLineID {  get; set; }
        public int PrescriptionID { get; set; }
        [ForeignKey("MedicineID")]
        public int MedicineID { get;set; }
        public string Instructions { get; set; }
        public int Quantity { get;set; }
        public int Repeats { get; set; }
        public int RepeatsLeft { get; set; }

    }
}
