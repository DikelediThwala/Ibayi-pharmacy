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
    }
}
