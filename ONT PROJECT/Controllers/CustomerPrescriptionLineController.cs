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

        /// <summary>
        /// Displays all prescription lines for the logged-in customer.
        /// Safely handles missing navigation properties to avoid null exceptions.
        /// </summary>
        public async Task<IActionResult> MyPrescriptionLines()
        {
            // Get logged-in user ID
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized();

            int userId = int.Parse(userIdStr);

            // Fetch the customer including allergies
            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation)
                .Include(c => c.CustomerAllergies)
                    .ThenInclude(ca => ca.ActiveIngredient)
                .FirstOrDefaultAsync(c => c.CustomerNavigation.UserId == userId);

            if (customer == null)
                return NotFound();

            // Fetch prescription lines including medicines, prescriptions, and ingredients
            var lines = await _context.PrescriptionLines
                .Include(pl => pl.Medicine)
                    .ThenInclude(m => m.MedIngredients)
                        .ThenInclude(mi => mi.ActiveIngredient)
                .Include(pl => pl.Prescription)
                    .ThenInclude(p => p.Customer)
                        .ThenInclude(c => c.CustomerAllergies)
                            .ThenInclude(ca => ca.ActiveIngredient)
                .Where(pl => pl.Prescription != null && pl.Prescription.CustomerId == customer.CustomerId)
                .ToListAsync();

            // Filter out any lines with missing navigation properties to prevent null errors
            lines = lines.Where(pl => pl.Medicine != null && pl.Prescription != null).ToList();

            return View(lines);
        }

        /// <summary>
        /// Places an order for the selected prescription lines.
        /// Checks for allergies and handles optional repeats.
        /// </summary>
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
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized();

            int userId = int.Parse(userIdStr);

            // Fetch customer with allergies
            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation)
                .Include(c => c.CustomerAllergies)
                    .ThenInclude(ca => ca.ActiveIngredient)
                .FirstOrDefaultAsync(c => c.CustomerNavigation.UserId == userId);

            if (customer == null)
                return NotFound();

            // Fetch selected prescription lines
            var lines = await _context.PrescriptionLines
                .Include(pl => pl.Medicine)
                    .ThenInclude(m => m.MedIngredients)
                        .ThenInclude(mi => mi.ActiveIngredient)
                .Include(pl => pl.Prescription)
                .Where(pl => selectedLines.Contains(pl.PrescriptionLineId))
                .ToListAsync();

            // Filter out null navigation properties
            lines = lines.Where(pl => pl.Medicine != null && pl.Prescription != null).ToList();

            var blocked = new List<string>();
            var ordered = new List<string>();

            // Create new order
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
                // Allergy check
                var medicineIngredients = line.Medicine.MedIngredients
                    .Select(mi => mi.ActiveIngredient.Ingredients)
                    .ToList();

                bool allergyConflict = customer.CustomerAllergies
                    .Any(ca => medicineIngredients.Contains(ca.ActiveIngredient.Ingredients));

                if (allergyConflict)
                {
                    blocked.Add($"{line.Medicine.MedicineName} (Allergy conflict)");
                    continue;
                }

                // Optional repeats check (uncomment if needed)
                // if (line.RepeatsLeft == null || line.RepeatsLeft <= 0)
                // {
                //     blocked.Add($"{line.Medicine.MedicineName} (No repeats left)");
                //     continue;
                // }

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
                ordered.Add(line.Medicine.MedicineName);

                // Optional: decrement repeats
                // if (line.RepeatsLeft != null)
                //     line.RepeatsLeft -= 1;
            }

            if (!order.OrderLines.Any())
            {
                TempData["ErrorMessage"] = "No medications could be ordered: " + string.Join(", ", blocked);
                return RedirectToAction("MyPrescriptionLines");
            }

            order.Vat = order.TotalDue * 0.15;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            if (blocked.Any())
                TempData["ErrorMessage"] = "Some medications could not be ordered: " + string.Join(", ", blocked);

            TempData["SuccessMessage"] = "Order placed for: " + string.Join(", ", ordered);
            return RedirectToAction("OrderedMedication", "CustomerOrder");
        }
    }
}
