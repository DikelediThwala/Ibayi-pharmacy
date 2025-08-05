using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ONT_PROJECT.Controllers;
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
        public IActionResult Create()
        {
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Name");
            ViewData["PharmacyManagerId"] = new SelectList(_context.PharmacyManagers, "PharmacyManagerId", "Name");

            ViewBag.Medicines = _context.Medicines.ToList();
            return View();
        }

        // POST: BOrder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BOrder order, List<int> medicineIds, List<int> quantities)
        {
            if (ModelState.IsValid)
            {
                // Set automatic date placed
                order.DatePlaced = DateOnly.FromDateTime(DateTime.Now);

                order.Status = "Pending";

                // Save the order
                _context.BOrders.Add(order);
                await _context.SaveChangesAsync();

                // Save order lines
                for (int i = 0; i < medicineIds.Count; i++)
                {
                    var line = new BOrderLine
                    {
                        BOrderId = order.BOrderId,
                        MedicineId = medicineIds[i],
                        Quantity = quantities[i]
                    };
                    _context.BOrderLines.Add(line);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order placed successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Name", order.SupplierId);
            ViewData["PharmacyManagerId"] = new SelectList(_context.PharmacyManagers, "PharmacyManagerId", "Name", order.PharmacyManagerId);
            ViewBag.Medicines = _context.Medicines.ToList();

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsReceived(int id)
        {
            var order = await _context.BOrders.FindAsync(id);
            if (order == null)
                return NotFound();

            order.DateRecieved = DateOnly.FromDateTime(DateTime.Now);
            order.Status = "Received";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}































//// GET: Create order form - populate medications dropdown
//public IActionResult Create()
//{
//    ViewBag.Medications = _context.Medicines
//        .Select(m => new SelectListItem
//        {
//            Value = m.MedicineId.ToString(),
//            Text = m.MedicineName
//        })
//        .ToList();

//    return View(new BOrder());
//}

//// POST: Create order submit
//[HttpPost]
//[ValidateAntiForgeryToken]
//public async Task<IActionResult> Create(BOrder order)
//{
//    if (order == null || order.BOrderLines == null || !order.BOrderLines.Any())
//    {
//        ModelState.AddModelError("", "Please add at least one medication.");
//    }

//    if (ModelState.IsValid)
//    {
//        order.DatePlaced = System.DateOnly.FromDateTime(System.DateTime.Today);
//        order.Status = "Pending";
//        order.PharmacyManagerId = 1; // Set your actual manager id

//        var firstMed = _context.Medicines.FirstOrDefault(m => m.MedicineId == order.BOrderLines[0].MedicineId);
//        if (firstMed != null)
//        {
//            order.SupplierId = firstMed.SupplierId;
//        }

//        _context.BOrders.Add(order);
//        await _context.SaveChangesAsync();

//        return RedirectToAction(nameof(Index));
//    }

//    ViewBag.Medications = _context.Medicines
//        .Select(m => new SelectListItem
//        {
//            Value = m.MedicineId.ToString(),
//            Text = m.MedicineName
//        })
//        .ToList();

//    return View(order);
//}





