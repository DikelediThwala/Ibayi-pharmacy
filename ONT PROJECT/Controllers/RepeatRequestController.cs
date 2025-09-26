using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace ONT_PROJECT.Controllers
{
    [Authorize(Roles = "Customer")]
    public class RepeatRequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RepeatRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Show collected medications for requesting repeats
        public async Task<IActionResult> CollectedMedications()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized();

            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            // find the current customer
            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation)
                .FirstOrDefaultAsync(c => c.CustomerNavigation.UserId == userId);

            if (customer == null)
                return NotFound();

            // get all OrderLines from orders marked as "Received"
            var collectedLines = await _context.OrderLines
                .Include(ol => ol.Medicine)
                .Include(ol => ol.Order)
                .Where(ol => ol.Order.CustomerId == customer.CustomerId
                             && ol.Order.Status == "Received")
                .ToListAsync();

            return View(collectedLines);
        }

        // AJAX repeat request endpoint
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjaxRequestRepeat(int orderLineId)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
                return Json(new { success = false, message = "User not logged in." });

            if (!int.TryParse(userIdStr, out int userId))
                return Json(new { success = false, message = "Invalid user id." });

            // find the current customer
            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation)
                .FirstOrDefaultAsync(c => c.CustomerNavigation.UserId == userId);

            if (customer == null)
                return Json(new { success = false, message = "Customer not found." });

            // find the specific OrderLine the customer is requesting a repeat for
            var line = await _context.OrderLines
                .Include(ol => ol.Medicine)
                .Include(ol => ol.Order)
                .FirstOrDefaultAsync(ol => ol.OrderLineId == orderLineId
                                           && ol.Order.CustomerId == customer.CustomerId
                                           && ol.Order.Status == "Received");

            if (line == null)
                return Json(new { success = false, message = "Order line not found or not collected yet." });

            // create a RepeatRequest linked directly to OrderLineId
            var repeatRequest = new RepeatRequest
            {
                OrderLineId = line.OrderLineId,   // link directly to the order line
                RequestDate = DateTime.Now,
                Status = "Pending"
            };

            _context.RepeatRequest.Add(repeatRequest);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = $"Repeat requested for {line.Medicine?.MedicineName ?? "Unknown Medication"}."
            });
        }
    }
}
