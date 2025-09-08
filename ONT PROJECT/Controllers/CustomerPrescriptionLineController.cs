using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace ONT_PROJECT.Controllers
{
    public class CustomerPrescriptionLineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerPrescriptionLineController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyPrescriptionLines()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var userId = int.Parse(userIdStr);

            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation)
                .FirstOrDefaultAsync(c => c.CustomerNavigation.UserId == userId);

            if (customer == null) return NotFound();

            var lines = await _context.PrescriptionLines
                .Include(pl => pl.Prescription)
                .Where(pl => pl.Prescription.CustomerId == customer.CustomerId)
                .ToListAsync();

            return View(lines);
        }


        // New action to get the number of prescription lines for dashboard
        [Authorize(Roles = "Customer")]
        public async Task<int> GetPrescriptionLineCount()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
                return 0;

            int userId = int.Parse(userIdStr);

            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation)
                .FirstOrDefaultAsync(c => c.CustomerNavigation.UserId == userId);

            if (customer == null)
                return 0;

            int lineCount = await _context.PrescriptionLines
                .Include(pl => pl.Prescription)
                .Where(pl => pl.Prescription.CustomerId == customer.CustomerId)
                .CountAsync();

            return lineCount;
        }
    }
}
