using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

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
            // Get logged-in user Id
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized();

            int userId = int.Parse(userIdStr);

            // Get the customer for this user
            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation) // optional if you need navigation data
                .FirstOrDefaultAsync(c => c.CustomerNavigation.UserId == userId);

            if (customer == null)
                return NotFound();

            // Get all prescription lines for this customer and include the Medicine
            var lines = await _context.PrescriptionLines
                .Include(pl => pl.Medicine)        // load Medicine
                .Include(pl => pl.Prescription)    // load Prescription if needed
                .Where(pl => pl.Prescription.CustomerId == customer.CustomerId)
                .ToListAsync();

            return View(lines);
        }

        // Action to get the number of prescription lines for dashboard
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
