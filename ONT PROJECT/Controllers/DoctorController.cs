using IBayiLibrary.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using ONT_PROJECT.Helpers;
using ONT_PROJECT.Models; 
using System.Linq;

namespace ONT_PROJECT.Controllers
{
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var doctors = _context.Doctors.ToList();
            ViewBag.ActiveDoctors = _context.Doctors.Where(d => d.Status == "Active").ToList();
            ViewBag.DeactivatedDoctors = _context.Doctors.Where(d => d.Status == "Deactivated").ToList();
            return View();
        }

        public IActionResult Create()
        {
            var doctor = new ONT_PROJECT.Models.Doctor();
            return View(doctor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ONT_PROJECT.Models.Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                if (_context.Doctors.Any(d => d.PracticeNo == doctor.PracticeNo || d.Email.ToLower() == doctor.Email.ToLower()))
                {
                    TempData["ErrorMessage"] = "A doctor with this Practice Number or Email already exists!";
                    return View(doctor);
                }

                _context.Doctors.Add(doctor);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Doctor added successfully!";

                ActivityLogger.LogActivity(_context, "Create Doctor", $"Doctor {doctor.Name} was added to the system.");

                return RedirectToAction(nameof(Index));
            }
            return View(doctor);
        }

        public IActionResult Edit(int id)
        {
            var doctor = _context.Doctors.FirstOrDefault(d => d.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }
            return View(doctor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ONT_PROJECT.Models.Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                _context.Doctors.Update(doctor);
                _context.SaveChanges();


                TempData["SuccessMessage"] = "Doctor details updated successfully.";
                return RedirectToAction(nameof(Index), new { id = doctor.DoctorId });
            }


            return View(doctor);
        }

        [HttpPost]
        public IActionResult SoftDelete(int id)
        {
            var doctor = _context.Doctors.FirstOrDefault(d => d.DoctorId == id);
            if (doctor != null)
            {
                doctor.Status = "Deactivated";
                _context.SaveChanges();

                ActivityLogger.LogActivity(_context, "Deactivate doctor", $"Doctor {doctor.Name} was deactivated.");

                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        public IActionResult Activate(int id)
        {
            var doctor = _context.Doctors.FirstOrDefault(d => d.DoctorId == id);
            if (doctor != null)
            {
                doctor.Status = "Active";
                _context.SaveChanges();
                ActivityLogger.LogActivity(_context, "Activate Doctor", $"Doctor {doctor.Name} was activated.");

                return Ok();
            }
            return BadRequest();
        }


    }
}
