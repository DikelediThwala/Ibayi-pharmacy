using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;
using System.Collections.Generic;
using System.Linq;

namespace ONT_PROJECT.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var viewModel = new ManagerDashboardViewModel
            {
                RecentOrders = GetRecentOrders(),
                RecentActivities = GetRecentActivities(),
                TotalMedicines = GetTotalMedicines(),
                TotalSuppliers = GetTotalSuppliers(),
                TotalPharmacists = GetTotalPharmacists(),
                TotalDoctors = GetTotalDoctors(),
                LowStockCount = GetLowStockCount() 
            };

            return View(viewModel);
        }

        private List<BOrder> GetRecentOrders()
        {
            return _context.BOrders
                .Include(o => o.BOrderLines)
                    .ThenInclude(ol => ol.Medicine)
                .OrderByDescending(o => o.DatePlaced)
                .Take(5)
                .ToList();
        }

        private List<ActivityLog> GetRecentActivities()
        {
            return _context.ActivityLogs
                .OrderByDescending(a => a.DatePerformed)
                .Take(7)
                .ToList();
        }

        private int GetTotalMedicines() => _context.Medicines.Count(m => m.Status == "Active");
        private int GetTotalSuppliers() => _context.Suppliers.Count(s => s.Status == "Active");
        private int GetTotalPharmacists()
        {
            int varOcg = _context.TblUsers.Count(u => u.Role == "Pharmacist" && u.Status == "Active");
            return varOcg;
        }
        private int GetTotalDoctors() => _context.Doctors.Count(d=> d.Status == "Active");
        private int GetLowStockCount()
        {
            return _context.Medicines
                .Count(m => m.Quantity < m.ReorderLevel && m.Status == "Active");
        }
    }
}
