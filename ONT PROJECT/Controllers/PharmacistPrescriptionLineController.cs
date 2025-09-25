using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;
using System.Threading.Tasks;
using System.Linq;

namespace ONT_PROJECT.Controllers
{
    public class PharmacistPrescriptionLineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PharmacistPrescriptionLineController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: List all unprocessed prescriptions
        public async Task<IActionResult> Index()
        {
            var prescriptions = await _context.UnprocessedPrescriptions
                .Include(u => u.Customer)
                .ThenInclude(c => c.CustomerNavigation)
                .AsNoTracking()
                .ToListAsync();

            return View(prescriptions);
        }

        // GET: AddLine form
        public async Task<IActionResult> AddLine(int id)
        {
            var prescription = await _context.UnprocessedPrescriptions
                .Include(u => u.Customer)
                .ThenInclude(c => c.CustomerNavigation)
                .FirstOrDefaultAsync(u => u.UnprocessedPrescriptionId == id);

            if (prescription == null) return NotFound();

            ViewBag.Medicines = await _context.Medicines.ToListAsync();
            return View(prescription);
        }

        [HttpPost]
        public async Task<IActionResult> AddLine(int id, int medicineId, int quantity, string instructions, int? repeats)
        {
            var prescription = await _context.UnprocessedPrescriptions.FindAsync(id);
            if (prescription == null) return NotFound();

            var prescriptionLine = new PrescriptionLine
            {
                PrescriptionId = 0, // or link to Prescription if created
                MedicineId = medicineId,
                Quantity = quantity,
                Instructions = instructions,
                Repeats = repeats,
                RepeatsLeft = repeats,
                Date = DateOnly.FromDateTime(DateTime.Now)
            };

            _context.PrescriptionLines.Add(prescriptionLine);

            // Mark the unprocessed prescription as processed
            prescription.Status = "Processed";
            _context.UnprocessedPrescriptions.Update(prescription);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
