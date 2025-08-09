using Microsoft.AspNetCore.Mvc;
using ONT_PROJECT.Models;
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

        // GET: List all prescription lines
        public IActionResult Index()
        {
            var prescriptionLines = _context.PrescriptionLines.ToList(); // ✅ Correct DbSet name
            return View(prescriptionLines);
        }

        // GET: Create view
        public IActionResult Create()
        {
            var prescriptionLine = new PrescriptionLine(); // ✅ Correct type
            return View(prescriptionLine);
        }

        // POST: Create new prescription line
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PrescriptionLine prescriptionLine)
        {
            if (ModelState.IsValid)
            {
                _context.PrescriptionLines.Add(prescriptionLine); // ✅ Correct DbSet
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Prescription line added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(prescriptionLine);
        }

        // GET: Edit view
        public IActionResult Edit(int id)
        {
            var prescriptionLine = _context.PrescriptionLines.FirstOrDefault(pl => pl.PrescriptionLineId == id); // ✅ Correct find
            if (prescriptionLine == null)
            {
                return NotFound();
            }
            return View(prescriptionLine);
        }

        // POST: Save edited prescription line
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(PrescriptionLine prescriptionLine)
        {
            if (ModelState.IsValid)
            {
                _context.PrescriptionLines.Update(prescriptionLine); // ✅ Correct DbSet
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(prescriptionLine);
        }
    }
}
