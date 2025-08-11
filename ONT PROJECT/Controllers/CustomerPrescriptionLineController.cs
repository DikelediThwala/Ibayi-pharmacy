using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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


        public IActionResult Create(int prescriptionId)
        {
            ViewBag.Medicines = new SelectList(_context.Medicines, "MedicineId", "MedicineName");
            var model = new PrescriptionLine { PrescriptionId = prescriptionId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PrescriptionLine prescriptionLine)
        {
            if (prescriptionLine.PrescriptionId == 0)
            {
                ModelState.AddModelError("PrescriptionId", "Prescription ID is required.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Medicines = new SelectList(_context.Medicines, "MedicineId", "MedicineName", prescriptionLine.MedicineId);
                return View(prescriptionLine);
            }

            _context.PrescriptionLines.Add(prescriptionLine);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Prescription line added successfully!";
            return RedirectToAction(nameof(Index));
        }



        // GET: Edit view
        public IActionResult Edit(int id)
        {
            var prescriptionLine = _context.PrescriptionLines.FirstOrDefault(pl => pl.PrescriptionLineId == id);
            if (prescriptionLine == null)
            {
                return NotFound();
            }

            ViewBag.Medicines = new SelectList(_context.Medicines, "MedicineId", "MedicineName", prescriptionLine.MedicineId);

            return View(prescriptionLine);
        }

        // POST Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(PrescriptionLine prescriptionLine)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.PrescriptionLines.Update(prescriptionLine);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again.");
                    // optionally log ex
                }
            }

            // repopulate dropdown on failure
            ViewBag.Medicines = new SelectList(_context.Medicines, "MedicineId", "MedicineName", prescriptionLine.MedicineId);

            return View(prescriptionLine);
        }
        // GET: Delete confirmation page
        public IActionResult Delete(int id)
        {
            var prescriptionLine = _context.PrescriptionLines.FirstOrDefault(pl => pl.PrescriptionLineId == id);
            if (prescriptionLine == null)
            {
                return NotFound();
            }
            return View(prescriptionLine);
        }
        // POST: Delete the prescription line
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var prescriptionLine = _context.PrescriptionLines.Find(id);
            if (prescriptionLine == null)
            {
                return NotFound();
            }

            _context.PrescriptionLines.Remove(prescriptionLine);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Prescription line deleted successfully!";
            return RedirectToAction(nameof(Index));
        }


    }
}

