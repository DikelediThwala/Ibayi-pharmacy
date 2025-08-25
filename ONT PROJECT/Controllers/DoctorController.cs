using Microsoft.AspNetCore.Mvc;
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
            return View(doctors);
        }

        public IActionResult Create()
        {
            var doctor = new Doctor();
            return View(doctor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                // Check if doctor already exists
                bool exists = _context.Doctors.Any(d =>
                    d.Name.ToLower() == doctor.Name.ToLower() &&
                    d.Surname.ToLower() == doctor.Surname.ToLower() &&
                    d.PracticeNo.ToLower() == doctor.PracticeNo.ToLower()
                );

                if (exists)
                {
                    TempData["ErrorMessage"] = "Doctor already exists in the system!";
                    return RedirectToAction(nameof(Create));
                }
                _context.Doctors.Add(doctor);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Doctor added successfully!";

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
        public IActionResult Edit(Doctor doctor)
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


    }
}
