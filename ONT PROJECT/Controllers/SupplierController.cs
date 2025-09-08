using IBayiLibrary.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                // Check if doctor already exists
                bool exists = _context.Suppliers.Any(d =>
                    d.Name.ToLower() == supplier.Name.ToLower() 
                 );

                if (exists)
                {
                    TempData["ErrorMessage"] = "Supplier already exists in the system!";
                    return RedirectToAction(nameof(Create));
                }
                _context.Suppliers.Add(supplier);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Supplier added successfully!";
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
        public IActionResult Delete(int id, Dictionary<int, int> reassignments)
        {
            var supplier = _context.Suppliers.FirstOrDefault(s => s.SupplierId == id);
            if (supplier == null) return NotFound();

            var meds = _context.Medicines.Where(m => m.SupplierId == id).ToList();
            foreach (var med in meds)
            {
                if (reassignments != null && reassignments.ContainsKey(med.MedicineId))
                    med.SupplierId = reassignments[med.MedicineId];
            }

            supplier.Status = "Deactivated";
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Supplier deactivated and medications reassigned!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Activate(int id)
        {
            var supplier = _context.Suppliers.FirstOrDefault(s => s.SupplierId == id);
            if (supplier == null) return NotFound();

            supplier.Status = "Active";
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Supplier activated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetSupplierMedications(int id)
        {
            var meds = _context.Medicines
                .Where(m => m.SupplierId == id && m.Status == "Active")
                .Select(m => new { m.MedicineId, m.MedicineName }).ToList();

            var otherSuppliers = _context.Suppliers
                .Where(s => s.SupplierId != id && s.Status == "Active")
                .Select(s => new { s.SupplierId, s.Name }).ToList();

            return Json(new { meds, otherSuppliers });
        }
    }
}
