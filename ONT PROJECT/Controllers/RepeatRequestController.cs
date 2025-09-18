using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;
using System.Security.Claims;

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

            int userId = int.Parse(userIdStr);

            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation)
                .FirstOrDefaultAsync(c => c.CustomerNavigation.UserId == userId);

            if (customer == null)
                return NotFound();

            // Fetch prescription lines safely with null check
            var collectedLines = await _context.PrescriptionLines
                .Include(pl => pl.Medicine)
                .Include(pl => pl.Prescription)
                .Where(pl => pl.Prescription != null
                             && pl.Prescription.CustomerId == customer.CustomerId
                             && pl.Prescription.Status == "Collected")
                .ToListAsync();

            return View(collectedLines);
        }

        // Request a repeat
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestRepeat(int prescriptionLineId)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdStr);

            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation)
                .FirstOrDefaultAsync(c => c.CustomerNavigation.UserId == userId);

            if (customer == null)
                return NotFound();

            var line = await _context.PrescriptionLines
                .Include(pl => pl.Medicine)
                .Include(pl => pl.Prescription)
                .FirstOrDefaultAsync(pl => pl.PrescriptionLineId == prescriptionLineId);

            if (line == null)
            {
                TempData["ErrorMessage"] = "Prescription line not found.";
                return RedirectToAction("CollectedMedications");
            }

            if (line.RepeatsLeft <= 0)
            {
                TempData["ErrorMessage"] = $"No repeats left for {line.Medicine?.MedicineName ?? "Unknown Medication"}.";
                return RedirectToAction("CollectedMedications");
            }

            // Decrement repeats
            line.RepeatsLeft -= 1;

            // Optional: create repeat record if you want to track requests
            var repeatRequest = new RepeatRequest
            {
                PrescriptionLineId = line.PrescriptionLineId,
                RequestDate = DateTime.Now,
                Status = "Pending"
            };

            _context.RepeatRequests.Add(repeatRequest);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Repeat requested for {line.Medicine?.MedicineName ?? "Unknown Medication"}.";
            return RedirectToAction("CollectedMedications");
        }
    }
}
