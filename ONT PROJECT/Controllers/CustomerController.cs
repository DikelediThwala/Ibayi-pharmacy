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
            // 1️⃣ Get logged-in user ID from claims
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdStr);

            // 2️⃣ Find Customer linked to this user
            var customer = await _context.Customers
                .Include(c => c.Prescriptions)
                .FirstOrDefaultAsync(c => c.CustomerNavigation.UserId == userId);

            if (customer == null)
                return NotFound();

            // 3️⃣ Fetch counts for dashboard
            var prescriptionLineCount = await _context.PrescriptionLines
                .Include(pl => pl.Prescription)
                .CountAsync(pl => pl.Prescription.CustomerId == customer.CustomerId);

            var orderCount = await _context.Orders
                .CountAsync(o => o.CustomerId == customer.CustomerId);

            // 4️⃣ Pass data to the view
            ViewBag.User = await _context.TblUsers.FindAsync(userId);
            ViewBag.PrescriptionLineCount = prescriptionLineCount;
            ViewBag.RepeatCount = 0; // leave placeholder for now
            ViewBag.OrderCount = orderCount;

            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
