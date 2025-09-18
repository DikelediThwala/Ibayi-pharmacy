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

            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation)
                .FirstOrDefaultAsync(c => c.CustomerNavigation.UserId == userId);

            if (customer == null)
                return NotFound();

            var lines = await _context.PrescriptionLines
                .Include(pl => pl.Medicine)
                .Include(pl => pl.Prescription)
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

            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation)
                .FirstOrDefaultAsync(c => c.CustomerNavigation.UserId == userId);

            if (customer == null)
                return NotFound();

            var order = new Order
            {
                CustomerId = customer.CustomerId,
                Status = "Pending",
                DatePlaced = DateOnly.FromDateTime(DateTime.Now),
                TotalDue = 0,
                Vat = 0,
                OrderLines = new List<OrderLine>()
            };

            foreach (var lineId in selectedLines)
            {
                var pl = await _context.PrescriptionLines
                    .Include(p => p.Medicine)
                    .FirstOrDefaultAsync(x => x.PrescriptionLineId == lineId);

                if (pl != null && pl.Medicine != null)
                {
                    double price = pl.Medicine.SalesPrice;
                    order.OrderLines.Add(new OrderLine
                    {
                        LineId = pl.PrescriptionLineId,
                        MedicineId = pl.MedicineId,
                        Quantity = pl.Quantity,
                        Price = price,
                        LineTotal = price * pl.Quantity,
                        Status = "Pending"
                    });

                    order.TotalDue += price * pl.Quantity;
                }
            }

            order.Vat = order.TotalDue * 0.15; // 15% VAT

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order placed successfully!";

            // Redirect to the OrderedMedication view in CustomerOrderController
            return RedirectToAction("OrderedMedication", "CustomerOrder");
        }
    }
}
