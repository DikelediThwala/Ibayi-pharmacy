using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;
using System.Security.Claims;

namespace ONT_PROJECT.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Show prescription lines for ordering
        public async Task<IActionResult> Order()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            // Get customer record
            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation)
                .FirstOrDefaultAsync(c => c.CustomerNavigation.UserId == userId);

            if (customer == null) return NotFound();

            // Get prescription lines for this customer
            var prescriptionLines = await _context.PrescriptionLines
                .Include(pl => pl.Medicine)
                .Include(pl => pl.Prescription)
                .Where(pl => pl.Prescription.CustomerId == customer.CustomerId)
                .ToListAsync();

            return View(prescriptionLines);
        }

        // Place order for selected prescription lines
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(int[] selectedLines)
        {
            if (selectedLines == null || selectedLines.Length == 0)
            {
                TempData["Error"] = "No medications selected.";
                return RedirectToAction("Order");
            }

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation)
                .FirstOrDefaultAsync(c => c.CustomerNavigation.UserId == userId);

            if (customer == null) return NotFound();

            var order = new Order
            {
                CustomerId = customer.CustomerId,
                Status = "Ordered",
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
                        Status = "Ordered"
                    });

                    order.TotalDue += price * pl.Quantity;
                }
            }

            order.Vat = order.TotalDue * 0.15; // 15% VAT

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Order placed successfully!";
            return RedirectToAction("OrderedMedication");
        }

        // Show all customer orders
        public async Task<IActionResult> OrderedMedication()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation)
                .FirstOrDefaultAsync(c => c.CustomerNavigation.UserId == userId);

            if (customer == null) return NotFound();

            var orders = await _context.Orders
                .Include(o => o.OrderLines)
                    .ThenInclude(ol => ol.Medicine)
                .Where(o => o.CustomerId == customer.CustomerId)
                .ToListAsync();

            return View(orders);
        }

        // Delete a specific order (and its lines)
        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderLines)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("OrderedMedication");
            }

            _context.OrderLines.RemoveRange(order.OrderLines);
            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Order deleted successfully!";
            return RedirectToAction("OrderedMedication");
        }

        // Mark an order as received
        [HttpPost]
        public async Task<IActionResult> MarkAsReceived(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
                return Json(new { success = false, message = "Order not found" });

            order.Status = "Received";
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Order marked as received." });
        }
    }
}
