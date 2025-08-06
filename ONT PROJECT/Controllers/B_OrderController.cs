using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
            SeedMedicines();  
        }
        public IActionResult Index()
        {
            var vm = new NewOrderViewModel
            {
                Medicines = _context.Medicines.ToList(),
                BOrders = _context.BOrders
                     .Include(o => o.BOrderLines)           
                     .ThenInclude(ol => ol.Medicine)       
                     .ToList(),
                NewOrder = new BOrder()
            };
            return View(vm);      
        }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BOrder order)
        {
            ModelState.Remove("Status");

            if (!ModelState.IsValid)
            {
                var allErrors = ModelState
                    .SelectMany(kvp => kvp.Value.Errors.Select(e => new { Field = kvp.Key, Error = e.ErrorMessage }))
                    .ToList();

                return BadRequest(new { success = false, message = "Validation errors occurred.", errors = allErrors });
            }

            if (order == null || order.BOrderLines == null || !order.BOrderLines.Any())
            {
                return BadRequest(new { success = false, message = "Please add at least one medication." });
            }

            order.DatePlaced = DateOnly.FromDateTime(DateTime.Now);
            order.Status = "Pending";

            _context.BOrders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Order placed successfully!" });
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

            // ✅ Build the full view model
            var viewModel = new NewOrderViewModel
            {
                Medicines = _context.Medicines.ToList(),
                BOrders = _context.BOrders
    .Include(o => o.BOrderLines)
    .ThenInclude(ol => ol.Medicine)
    .ToList(),
                NewOrder = new BOrder()
            };

            // ✅ Pass tab=orders so the correct tab is active
            ViewData["ActiveTab"] = "orders";

            return View("Index", viewModel);
        }


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

    }
}
