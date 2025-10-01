using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;
using System.Security.Claims;

namespace ONT_PROJECT.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdStr);

            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation)
                .FirstOrDefaultAsync(c => c.CustomerNavigation != null && c.CustomerNavigation.UserId == userId);

            if (customer == null)
                return NotFound();

            // Counts
            var prescriptionLineCount = await _context.PrescriptionLines
                .Include(pl => pl.Prescription)
                .CountAsync(pl => pl.Prescription.CustomerId == customer.CustomerId);

            var orderCount = await _context.Orders
                .CountAsync(o => o.CustomerId == customer.CustomerId);

            // Repeats (grouped by medicine)
            var repeatCounts = await _context.RepeatRequest
                .Include(r => r.OrderLine)
                    .ThenInclude(ol => ol.Medicine)
                .Include(r => r.OrderLine)
                    .ThenInclude(ol => ol.Order)
                .Where(r => r.OrderLine.Order.CustomerId == customer.CustomerId)
                .GroupBy(r => r.OrderLine.Medicine.MedicineName)
                .Select(g => new RepeatCountViewModel
                {
                    MedicineName = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Recent Orders (last 5)
            var recentOrders = await _context.Orders
                .Where(o => o.CustomerId == customer.CustomerId)
                .OrderByDescending(o => o.DatePlaced)
                .Take(5)
                .Include(o => o.OrderLines)
                    .ThenInclude(ol => ol.Medicine)
                .Select(o => new CustomerOrderViewModel
                {
                    OrderId = o.OrderId,
                    DatePlaced = o.DatePlaced.ToDateTime(TimeOnly.MinValue), // convert DateOnly -> DateTime
                    Status = o.Status,
                    OrderLines = o.OrderLines.Select(ol => new CustomerOrderLineViewModel
                    {
                        MedicineName = ol.Medicine.MedicineName
                    }).ToList()
                }).ToListAsync();

            var model = new CustomerDashboardViewModel
            {
                User = await _context.TblUsers.FindAsync(userId),
                PrescriptionLineCount = prescriptionLineCount,
                OrderCount = orderCount,
                RepeatCounts = repeatCounts,
                RecentOrders = recentOrders
            };

            return View(model);
        }
    }
}
