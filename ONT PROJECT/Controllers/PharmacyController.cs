using IBayiLibrary.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Helpers;
using ONT_PROJECT.Models;
using System.IO;

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
            var pharmacy = _context.Pharmacies
                .Include(p => p.Pharmacist) 
                    .ThenInclude(ph => ph.PharmacistNavigation) 
                .FirstOrDefault();

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
            var pharmacy = _context.Pharmacies
                .Include(p => p.Pharmacist)
                    .ThenInclude(ph => ph.PharmacistNavigation) 
                .FirstOrDefault();

            if (pharmacy == null)
            {
                TempData["ErrorMessage"] = "No pharmacy record found.";
                return RedirectToAction("Index");
            }

            var activePharmacists = _context.Pharmacists
                .Include(p => p.PharmacistNavigation)
                .Where(p => p.PharmacistNavigation.Status == "Active") 
                .Select(p => new
                {
                    PharmacistId = p.PharmacistId,
                    FullName = p.PharmacistNavigation.FirstName + " " + p.PharmacistNavigation.LastName
                })
                .OrderBy(p => p.FullName)
                .ToList();

            ViewBag.PharmacistList = new SelectList(
                activePharmacists,
                "PharmacistId",
                "FullName",
                pharmacy.PharmacistId 
            );

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
                ActivityLogger.LogActivity(_context, "Activate Pharmacy", $"Pharmacy details were updated.");

                return RedirectToAction("Index");

            }

            
            ViewBag.PharmacistList = new SelectList(
                _context.Pharmacists.Include(p => p.PharmacistNavigation)
                    .OrderBy(p => p.PharmacistId)
                    .Select(p => new
                    {
                        PharmacistId = p.PharmacistId,
                        FullName = p.PharmacistNavigation.FirstName + " " + p.PharmacistNavigation.LastName
                    }),
                "PharmacistId",
                "FullName",
                pharmacyModel.PharmacistId
            );

            return View(pharmacyModel);
        }

    }
}
