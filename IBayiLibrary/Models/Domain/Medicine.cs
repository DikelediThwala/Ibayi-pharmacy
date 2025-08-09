using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Models.Domain
{
    public class Medicine
    {
        public int MedicineID { get; set; }

        public string MedicineName { get; set; }

        public int Schedule { get; set; }

        public double SalesPrice { get; set; }

        public int SupplierId { get; set; }

        public int ReorderLevel { get; set; }

        public int Quantity { get; set; }
        public int FormID { get; set; }
    }
}
