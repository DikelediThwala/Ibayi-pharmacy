using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ONT_PROJECT.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ONT_PROJECT.Controllers
{
    public class B_OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public B_OrderController(ApplicationDbContext context)
        {
            _context = context;
            SeedMedicines();  // Ensure medicines exist
        }

        // Seed medicines if none exist in DB
        private void SeedMedicines()
        {
            if (!_context.Medicines.Any())
            {
                _context.Medicines.AddRange(new List<Medicine>
                {
                    new Medicine { MedicineName = "Paracetamol" },
                    new Medicine { MedicineName = "Ibuprofen" }
                });
                _context.SaveChanges();
            }
        }

        public IActionResult TestMedicines()
        {
            int count = _context.Medicines.Count();
            return Content($"Medicines count: {count}");
        }

        public IActionResult Index()
        {
            var vm = new NewOrderViewModel
            {
                Medicines = _context.Medicines.ToList(),
                BOrders = _context.BOrders.ToList(),
                NewOrder = new BOrder()
            };
            return View(vm);
        }

        // GET: Create order form - populate medications dropdown
        public IActionResult Create()
        {
            ViewBag.Medications = _context.Medicines
                .Select(m => new SelectListItem
                {
                    Value = m.MedicineId.ToString(),
                    Text = m.MedicineName
                })
                .ToList();

            return View(new BOrder());
        }

        // POST: Create order submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BOrder order)
        {
            if (order == null || order.BOrderLines == null || !order.BOrderLines.Any())
            {
                ModelState.AddModelError("", "Please add at least one medication.");
            }

            if (ModelState.IsValid)
            {
                order.DatePlaced = System.DateOnly.FromDateTime(System.DateTime.Today);
                order.Status = "Pending";
                order.PharmacyManagerId = 1; // Set your actual manager id

                var firstMed = _context.Medicines.FirstOrDefault(m => m.MedicineId == order.BOrderLines[0].MedicineId);
                if (firstMed != null)
                {
                    order.SupplierId = firstMed.SupplierId;
                }

                _context.BOrders.Add(order);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Medications = _context.Medicines
                .Select(m => new SelectListItem
                {
                    Value = m.MedicineId.ToString(),
                    Text = m.MedicineName
                })
                .ToList();

            return View(order);
        }
    }
}
