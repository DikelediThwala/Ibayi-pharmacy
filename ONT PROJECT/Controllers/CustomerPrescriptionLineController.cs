using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;
using System.Security.Claims;

namespace ONT_PROJECT.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerPrescriptionLineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerPrescriptionLineController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Show prescription lines for the logged-in customer
        public async Task<IActionResult> MyPrescriptionLines()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized();

            int userId = int.Parse(userIdStr);

            // Get customer including allergies
            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation)
                .Include(c => c.CustomerAllergies)
                    .ThenInclude(ca => ca.ActiveIngredient)
                .FirstOrDefaultAsync(c => c.CustomerNavigation.UserId == userId);

            if (customer == null)
                return NotFound();

            // Get prescription lines including medicine ingredients and active ingredients
            var lines = await _context.PrescriptionLines
                .Include(pl => pl.Medicine)
                    .ThenInclude(m => m.MedIngredients)
                        .ThenInclude(mi => mi.ActiveIngredient)
                .Include(pl => pl.Prescription)
                    .ThenInclude(p => p.Customer)
                        .ThenInclude(c => c.CustomerAllergies)
                            .ThenInclude(ca => ca.ActiveIngredient)
                .Where(pl => pl.Prescription.CustomerId == customer.CustomerId)
                .ToListAsync();

            return View(lines);
        }

        // Place order for selected prescription lines
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(int[] selectedLines)
        {
            if (selectedLines == null || selectedLines.Length == 0)
            {
                TempData["ErrorMessage"] = "No medications selected. Please select at least one.";
                return RedirectToAction("MyPrescriptionLines");
            }

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdStr);

            // Get customer with allergies
            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation)
                .Include(c => c.CustomerAllergies)
                    .ThenInclude(ca => ca.ActiveIngredient)
                .FirstOrDefaultAsync(c => c.CustomerNavigation.UserId == userId);

            if (customer == null)
                return NotFound();

            // Get all selected prescription lines with medicines and ingredients
            var lines = await _context.PrescriptionLines
                .Include(pl => pl.Medicine)
                    .ThenInclude(m => m.MedIngredients)
                        .ThenInclude(mi => mi.ActiveIngredient)
                .Include(pl => pl.Prescription)
                .Where(pl => selectedLines.Contains(pl.PrescriptionLineId))
                .ToListAsync();

            // Ensure each prescription has a Date
            foreach (var line in lines)
            {
                if (line.Prescription.Date == default)
                {
                    line.Prescription.Date = DateOnly.FromDateTime(DateTime.Now);
                }
            }

            // Check for allergy conflicts
            foreach (var line in lines)
            {
                var medicineIngredients = line.Medicine.MedIngredients
                    .Select(mi => mi.ActiveIngredient.Ingredients)
                    .ToList();

                var allergyConflict = customer.CustomerAllergies
                    .Any(ca => medicineIngredients.Contains(ca.ActiveIngredient.Ingredients));

                if (allergyConflict)
                {
                    TempData["ErrorMessage"] = $"Cannot order {line.Medicine.MedicineName}. It contains ingredients you are allergic to.";
                    return RedirectToAction("MyPrescriptionLines");
                }
            }

            // Create order
            var order = new Order
            {
                CustomerId = customer.CustomerId,
                Status = "Pending",
                DatePlaced = DateOnly.FromDateTime(DateTime.Now),
                TotalDue = 0,
                Vat = 0,
                OrderLines = new List<OrderLine>()
            };

            foreach (var line in lines)
            {
                double price = line.Medicine.SalesPrice;
                order.OrderLines.Add(new OrderLine
                {
                    LineId = line.PrescriptionLineId,
                    MedicineId = line.MedicineId,
                    Quantity = line.Quantity,
                    Price = price,
                    LineTotal = price * line.Quantity,
                    Status = "Pending"
                });

                order.TotalDue += price * line.Quantity;
            }

            order.Vat = order.TotalDue * 0.15; // 15% VAT

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order placed successfully!";
            return RedirectToAction("OrderedMedication", "CustomerOrder");
        }
    }
}
