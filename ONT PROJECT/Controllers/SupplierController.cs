using IBayiLibrary.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Helpers;
using ONT_PROJECT.Models;

namespace ONT_PROJECT.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupplierController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var suppliers = _context.Suppliers.ToList();
            return View(suppliers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                bool exists = _context.Suppliers.Any(d =>
                  d.Name.ToLower() == supplier.Name.ToLower() ||
                  d.Email.ToLower() == supplier.Email.ToLower()
                );

                if (exists)
                {
                    TempData["ErrorMessage"] = "Supplier already exists in the system!";
                    return RedirectToAction(nameof(Create));
                }

                _context.Suppliers.Add(supplier);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Supplier added successfully!";
                ActivityLogger.LogActivity(_context, "Create Supplier", $"Supplier {supplier.Name} was added to the system.");

                return RedirectToAction(nameof(Index));
            }

            return View(supplier);
        }

        public IActionResult Edit(int id)
        {
            var supplier = _context.Suppliers.FirstOrDefault(s => s.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Suppliers.Update(supplier);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Supplier updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(supplier);
        }

        [HttpPost]
        public IActionResult Delete(int id, IFormCollection form)
        {
            var supplier = _context.Suppliers.FirstOrDefault(s => s.SupplierId == id);
            if (supplier == null)
                return NotFound();

            var reassignments = new Dictionary<int, int>();
            foreach (var key in form.Keys)
            {
                if (key.StartsWith("reassignments["))
                {
                    var medIdStr = key.Replace("reassignments[", "").Replace("]", "");
                    if (int.TryParse(medIdStr, out int medId) && int.TryParse(form[key], out int newSupplierId))
                    {
                        reassignments[medId] = newSupplierId;
                    }
                }
            }

            var meds = _context.Medicines.Where(m => m.SupplierId == id).ToList();
            foreach (var med in meds)
            {
                if (reassignments.ContainsKey(med.MedicineId))
                    med.SupplierId = reassignments[med.MedicineId];
            }

            supplier.Status = "Deactivated";
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Supplier deactivated successfully!";
            ActivityLogger.LogActivity(_context, "Deactivate Supplier", $"Supplier {supplier.Name} was deactivated.");

            return Ok();
        }

        [HttpPost]
        public IActionResult Activate(int id)
        {
            var supplier = _context.Suppliers.FirstOrDefault(s => s.SupplierId == id);
            if (supplier == null)
                return NotFound();

            supplier.Status = "Active";
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Supplier activated successfully!";
            ActivityLogger.LogActivity(_context, "Activate Supplier", $"Supplier {supplier.Name} was activated.");

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetSupplierMedications(int id)
        {
            var meds = _context.Medicines
                .Where(m => m.SupplierId == id && m.Status == "Active")
                .Select(m => new
                {
                    m.MedicineId,
                    m.MedicineName
                })
                .ToList();

            var otherSuppliers = _context.Suppliers
                .Where(s => s.SupplierId != id && s.Status == "Active")
                .Select(s => new
                {
                    supplierId = s.SupplierId,
                    name = s.Name
                })
                .ToList();

            return Json(new { meds, otherSuppliers });
        }
    }
}

