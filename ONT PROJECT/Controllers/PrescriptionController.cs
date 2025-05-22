using Microsoft.AspNetCore.Mvc;

namespace ONT_PROJECT.Controllers
{
    public class Prescription : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }

    }
}
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using ONT_PROJECT.Models;
//using System;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ONT_PROJECT.Controllers
//{
//    public class PrescriptionController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public PrescriptionController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // GET: Prescription
//        public async Task<IActionResult> Index()
//        {
//            int customerId = GetCurrentCustomerId();

//            var prescriptions = await _context.Prescriptions
//                .Where(p => p.CustomerID == customerId)
//                .ToListAsync();

//            return View(prescriptions);
//        }

//        // GET: Prescription/Create
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // POST: Prescription/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(int pharmacistId, int doctorId, DateOnly date, IFormFile prescriptionFile)
//        {
//            if (prescriptionFile == null || prescriptionFile.Length == 0)
//            {
//                ModelState.AddModelError("PrescriptionPhoto", "Please upload a prescription PDF file.");
//                return View();
//            }

//            if (!prescriptionFile.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
//            {
//                ModelState.AddModelError("PrescriptionPhoto", "Only PDF files are allowed.");
//                return View();
//            }

//            byte[] fileData;
//            using (var ms = new MemoryStream())
//            {
//                await prescriptionFile.CopyToAsync(ms);
//                fileData = ms.ToArray();
//            }

//            var prescription = new Prescription
//            {
//                CustomerID = GetCurrentCustomerId(),
//                PharmacistID = pharmacistId,
//                DoctorID = doctorId,
//                Date = date,
//                PrescriptionPhoto = fileData
//            };

//            _context.Prescriptions.Add(prescription);
//            await _context.SaveChangesAsync();

//            return RedirectToAction(nameof(Index));
//        }

//        // POST: Prescription/Delete/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var prescription = await _context.Prescriptions.FindAsync(id);
//            if (prescription == null || prescription.CustomerID != GetCurrentCustomerId())
//            {
//                return NotFound();
//            }

//            _context.Prescriptions.Remove(prescription);
//            await _context.SaveChangesAsync();

//            return RedirectToAction(nameof(Index));
//        }

//        private int GetCurrentCustomerId()
//        {
//            // Replace this with real authentication logic
//            return 1;
//        }
//    }
//}
