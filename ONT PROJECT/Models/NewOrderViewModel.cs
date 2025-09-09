using Microsoft.AspNetCore.Mvc.Rendering;

namespace ONT_PROJECT.Models
{
    public class NewOrderViewModel
    {
        public List<Medicine> Medicines { get; set; }
        public List<BOrder> BOrders { get; set; }
        public BOrder NewOrder { get; set; }
        public List<Medicine> MedDetails { get; set; } = new List<Medicine>();

        public List<SelectListItem> MedicationSelectList => Medicines?.Select(m => new SelectListItem{Value = m.MedicineId.ToString(),Text = m.MedicineName}).ToList();
    }

}
