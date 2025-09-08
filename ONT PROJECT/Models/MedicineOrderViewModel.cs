namespace ONT_PROJECT.Models
{
    public class MedicineOrderViewModel
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = null!;
        public int Quantity { get; set; }
        public string SupplierName { get; set; } = null!;
        public double SalesPrice { get; set; }
    }

}
