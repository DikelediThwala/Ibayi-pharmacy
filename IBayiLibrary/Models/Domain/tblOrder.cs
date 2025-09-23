using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Models.Domain
{
    public class tblOrder
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public int MedicineID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MedicineName { get; set; }
        public string Status { get; set; }
        public int PharmacistID { get; set; }
        public int VAT { get; set; }
        public int TotalDue { get; set; }
        public int Quantity { get; set; }
        public int SalesPrice { get; set; }
        public int TotalSales { get; set; }
        public DateOnly DatePlaced { get; set; }
        public DateOnly DateRecieved { get; set; }
        
    }
}
