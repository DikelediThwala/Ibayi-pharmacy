using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ONT_PROJECT.Models;

namespace ONT_PROJECT.Controllers
{
    public class PharmacyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PharmacyController(ApplicationDbContext context)
        {
            _context = context;
        }
 public IActionResult Index()
        {
            var pharmacy = _context.Pharmacies.FirstOrDefault();

            if (pharmacy == null)
            {
                ViewBag.Message = "No pharmacy information available.";
                return View();
            }

            // DEBUG: See what's inside pharmacy
            Console.WriteLine($"Pharmacy Name: {pharmacy.PharmacyId}, Email: {pharmacy.Email}");

            return View(pharmacy);
        }
       

        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Edit()
        {
            var pharmacy = _context.Pharmacies.FirstOrDefault();
            if (pharmacy == null)
            {
                return NotFound();
            }

            // Populate ViewBag if Pharmacists exist
            ViewBag.Pharmacists = new SelectList(_context.Pharmacists, "PharmacistID", "FullName");

            return View(pharmacy);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Pharmacy model, IFormFile? logoFile)
        {
            if (ModelState.IsValid)
            {
                var existingPharmacy = _context.Pharmacies.FirstOrDefault();
                if (existingPharmacy == null)
                    return NotFound();

                // Update properties
                existingPharmacy.Name = model.Name;
                existingPharmacy.HealthCounsilRegistrationNo = model.HealthCounsilRegistrationNo;
                existingPharmacy.ContactNo = model.ContactNo;
                existingPharmacy.Email = model.Email;
                existingPharmacy.WebsiteUrl = model.WebsiteUrl;
                existingPharmacy.PharmacistId = model.PharmacistId;
                existingPharmacy.Address = model.Address;
                existingPharmacy.Vatrate = model.Vatrate;

                // Optional: Update logo if a new one was uploaded
                if (logoFile != null && logoFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        logoFile.CopyTo(ms);
                        existingPharmacy.Logo = ms.ToArray();
                    }
                }

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            // Re-populate dropdown if validation fails
            ViewBag.Pharmacists = new SelectList(_context.Pharmacists, "PharmacistID", "FullName");

            return View(model);
        }

    }
}
