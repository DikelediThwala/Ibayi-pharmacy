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
        // GET: List all prescription lines for a specific prescription
        public IActionResult Index(int? prescriptionId)
        {
            List<PrescriptionLine> prescriptionLines = new List<PrescriptionLine>();

            if (prescriptionId.HasValue && prescriptionId.Value > 0)
            {
                prescriptionLines = _context.PrescriptionLines
                    .Where(pl => pl.PrescriptionId == prescriptionId.Value)
                    .ToList();
            }

            ViewBag.PrescriptionId = prescriptionId ?? 0; // 0 or null means no selection

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

            // Redirect to Index and pass the prescriptionId to show correct prescription lines
            return RedirectToAction(nameof(Index), new { prescriptionId = prescriptionLine.PrescriptionId });
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
        public IActionResult SelectPrescription()
        {
            // Get all prescriptions to list for user to choose
            var prescriptions = _context.Prescriptions.ToList();  // Adjust your db context accordingly
            return View(prescriptions);
        }



    }
}

