namespace ONT_PROJECT.Models
{
    public class CustomerDashboardViewModel
    {
        public TblUser? User { get; set; }
        public int PrescriptionLineCount { get; set; }
        public int OrderCount { get; set; }
        public List<RepeatCountViewModel> RepeatCounts { get; set; } = new();
        public List<CustomerOrderViewModel> RecentOrders { get; set; } = new(); // only one
    }

    public class RepeatCountViewModel
    {
        public string MedicineName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class CustomerOrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime DatePlaced { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<CustomerOrderLineViewModel> OrderLines { get; set; } = new();
    }

    public class CustomerOrderLineViewModel
    {
        public string MedicineName { get; set; } = string.Empty;
    }
}
