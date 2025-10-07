namespace ONT_PROJECT.Models
{
    public class ManagerDashboardViewModel
    {
        public List<BOrder> RecentOrders { get; set; } = new List<BOrder>();
        public List<ActivityLog> RecentActivities { get; set; } = new List<ActivityLog>();

        // Info card counts
        public int TotalMedicines { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalPharmacists { get; set; }
        public int TotalDoctors { get; set; }
        public int LowStockCount { get; set; }

    }
}
