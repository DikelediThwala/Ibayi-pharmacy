using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;
using System.Security.Claims;

namespace ONT_PROJECT.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerPrescriptionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerPrescriptionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Displays all prescriptions (view-only)
        public async Task<IActionResult> MyPrescriptions()
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

            var prescriptions = await _context.Prescriptions
                .Include(p => p.Doctor)
                .Include(p => p.PrescriptionLines)
                    .ThenInclude(pl => pl.Medicine)
                        .ThenInclude(m => m.MedIngredients)
                            .ThenInclude(mi => mi.ActiveIngredient)
                .Where(p => p.CustomerId == customer.CustomerId)
                .ToListAsync();

            return View(prescriptions);
        }
    }
}
