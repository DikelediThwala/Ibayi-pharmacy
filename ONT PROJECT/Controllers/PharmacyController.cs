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
                TempData["ErrorMessage"] = "No pharmacy record found.";
                return RedirectToAction("Index");
            }

            ViewBag.Pharmacists = new SelectList(_context.Pharmacists, "PharmacistId", "FullName");

            return View(pharmacy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Pharmacy pharmacyModel, IFormFile? logoFile)
        {
            if (ModelState.IsValid)
            {
                var existingPharmacy = _context.Pharmacies.FirstOrDefault();
                if (existingPharmacy == null)
                    return NotFound();

                existingPharmacy.Name = pharmacyModel.Name;
                existingPharmacy.HealthCounsilRegistrationNo = pharmacyModel.HealthCounsilRegistrationNo;
                existingPharmacy.ContactNo = pharmacyModel.ContactNo;
                existingPharmacy.Email = pharmacyModel.Email;
                existingPharmacy.WebsiteUrl = pharmacyModel.WebsiteUrl;
                existingPharmacy.PharmacistId = pharmacyModel.PharmacistId;
                existingPharmacy.Address = pharmacyModel.Address;
                existingPharmacy.Vatrate = pharmacyModel.Vatrate;

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

            ViewBag.Pharmacists = new SelectList(_context.Pharmacists, "PharmacistID", "FullName");


            return View(pharmacyModel);
        }



    }
}
