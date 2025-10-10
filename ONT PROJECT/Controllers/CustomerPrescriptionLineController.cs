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

        public async Task<IActionResult> MyPrescriptionLines()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized();

            int userId = int.Parse(userIdStr);

            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation)
                .Include(c => c.CustomerAllergies)
                    .ThenInclude(ca => ca.ActiveIngredient)
                .FirstOrDefaultAsync(c => c.CustomerNavigation.UserId == userId);

            if (customer == null)
                return NotFound();

            // STEP 1: Load prescription lines safely
            var lines = await _context.PrescriptionLines
                .Include(pl => pl.Medicine)
                    .ThenInclude(m => m.MedIngredients)
                        .ThenInclude(mi => mi.ActiveIngredient)
                .Include(pl => pl.Prescription)
                    .ThenInclude(p => p.Doctor)
                .Where(pl => pl.Prescription.CustomerId == customer.CustomerId)
                .ToListAsync();

            // STEP 2: Load repeat histories
            var lineIds = lines.Select(l => l.PrescriptionLineId).ToList();
            var allHistories = await _context.RepeatHistories
                .Where(rh => lineIds.Contains(rh.PrescriptionLineId))
                .OrderByDescending(rh => rh.DateUsed)
                .ToListAsync();

            // STEP 3: Attach histories to lines safely
            foreach (var line in lines)
            {
                line.RepeatHistories = allHistories
                    .Where(rh => rh.PrescriptionLineId == line.PrescriptionLineId)
                    .ToList();

                // Initialize RepeatsLeft if null
                if (!line.RepeatsLeft.HasValue && line.Repeats.HasValue)
                {
                    line.RepeatsLeft = line.Repeats.Value;
                    _context.Update(line);
                }

                // Make sure related properties are not null to prevent view errors
                if (line.Medicine == null)
                    line.Medicine = new Medicine();

                if (line.Prescription == null)
                    line.Prescription = new Prescription();

                if (line.Prescription.Doctor == null)
                    line.Prescription.Doctor = new Doctor();

                if (line.RepeatHistories == null)
                    line.RepeatHistories = new List<RepeatHistory>();
            }

            await _context.SaveChangesAsync();
            return View(lines);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(int[] selectedLines)
        {
            if (selectedLines == null || selectedLines.Length == 0)
            {
                TempData["ErrorMessage"] = "No medications selected.";
                return RedirectToAction(nameof(MyPrescriptionLines));
            }

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized();

            int userId = int.Parse(userIdStr);

            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation)
                .Include(c => c.CustomerAllergies)
                    .ThenInclude(ca => ca.ActiveIngredient)
                .FirstOrDefaultAsync(c => c.CustomerNavigation.UserId == userId);

            if (customer == null)
                return NotFound();

            var lines = await _context.PrescriptionLines
                .Include(pl => pl.Medicine)
                    .ThenInclude(m => m.MedIngredients)
                        .ThenInclude(mi => mi.ActiveIngredient)
                .Include(pl => pl.Prescription)
                .Where(pl => selectedLines.Contains(pl.PrescriptionLineId))
                .ToListAsync();

            var blocked = new List<string>();
            var ordered = new List<string>();

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
                if (line.Medicine == null)
                {
                    blocked.Add("Unknown medicine (data error)");
                    continue;
                }

                // Allergy check
                var medicineIngredients = line.Medicine.MedIngredients?
                    .Select(mi => mi.ActiveIngredient?.Ingredients)
                    .Where(i => !string.IsNullOrEmpty(i))
                    .ToList() ?? new List<string>();

                bool allergyConflict = customer.CustomerAllergies?
                    .Any(ca => medicineIngredients.Contains(ca.ActiveIngredient?.Ingredients)) ?? false;

                if (allergyConflict)
                {
                    blocked.Add($"{line.Medicine.MedicineName} (Allergy conflict)");
                    continue;
                }

                // Check repeats
                if (!line.RepeatsLeft.HasValue || line.RepeatsLeft <= 0)
                {
                    blocked.Add($"{line.Medicine.MedicineName} (No repeats left)");
                    continue;
                }

                double price = line.Medicine.SalesPrice;

                order.OrderLines.Add(new OrderLine
                {
                    LineId = line.PrescriptionLineId,
                    MedicineId = line.MedicineId,
                    Quantity = line.Quantity,
                    Price = price,
                    Status = "Pending"
                });

                order.TotalDue += price;
                ordered.Add(line.Medicine.MedicineName);

                // Decrement repeats
                line.RepeatsLeft -= 1;
                _context.Update(line);

                // Add repeat history (linked to this specific line)
                var repeatHistory = new RepeatHistory
                {
                    PrescriptionLineId = line.PrescriptionLineId,
                    RepeatsDecremented = 1,
                    DateUsed = DateTime.Now
                };

                await _context.RepeatHistories.AddAsync(repeatHistory);
            }

            if (!order.OrderLines.Any())
            {
                TempData["ErrorMessage"] = "No medications could be ordered: " + string.Join(", ", blocked);
                return RedirectToAction(nameof(MyPrescriptionLines));
            }

            order.Vat = order.TotalDue * 0.15;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            if (blocked.Any())
                TempData["ErrorMessage"] = "Some medications could not be ordered: " + string.Join(", ", blocked);

            TempData["SuccessMessage"] = "Order placed for: " + string.Join(", ", ordered);
            return RedirectToAction(nameof(MyPrescriptionLines));
        }
    }
}
