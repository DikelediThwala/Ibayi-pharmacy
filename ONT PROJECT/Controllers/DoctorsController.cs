using IBayiLibrary.Models.Domain;
using IBayiLibrary.Repository;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace ONT_PROJECT.Controllers
{
    public class DoctorsController : Controller
    {

        private readonly IDoctorRepository _doctorRepository;
        

        public DoctorsController(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository; 
          
        }
        public IActionResult CreateDoctor()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDoctor(Doctor doctor)
        {
            try
            {
                bool emailExists = await _doctorRepository.CheckEmailExistsAsync(doctor.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "This email address is already registered.");
                    TempData["msg"] = "Email already exists!";
                    return View(doctor);
                }


                bool addPerson = await _doctorRepository.AddAsync(doctor);
                if (addPerson)
                {
                    TempData["msg"] = "Sucessfully Added";
                }
                else
                {
                    TempData["msg"] = "Could not add";
                }
               
            }
            catch (Exception ex)
            {
                TempData["msg"] = " Something went wrong!!!";
            }
            return RedirectToAction("CreatePrescForWalkins", "UploadPrescription");
        }
        public IActionResult CreateDoctorForImmediateDispense()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDoctorForImmediateDispense(Doctor doctor)
        {
            try
            {
                bool emailExists = await _doctorRepository.CheckEmailExistsAsync(doctor.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "This email address is already registered.");
                    TempData["msg"] = "Email already exists!";
                    return View(doctor);
                }


                bool addPerson = await _doctorRepository.AddAsync(doctor);
                if (addPerson)
                {
                    TempData["msg"] = "Sucessfully Added";
                }
                else
                {
                    TempData["msg"] = "Could not add";
                }

            }
            catch (Exception ex)
            {
                TempData["msg"] = " Something went wrong!!!";
            }
            return RedirectToAction("CreatePrescForImmediateDispense", "UploadPrescription");
        }
        public IActionResult CreateDoctorForPresc()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDoctorForPresc(Doctor doctor)
        {
            try
            {
                bool emailExists = await _doctorRepository.CheckEmailExistsAsync(doctor.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "This email address is already registered.");
                    TempData["msg"] = "Email already exists!";
                    return View(doctor);
                }


                bool addPerson = await _doctorRepository.AddAsync(doctor);
                if (addPerson)
                {
                    TempData["msg"] = "Sucessfully Added";
                }
                else
                {
                    TempData["msg"] = "Could not add";
                }

            }
            catch (Exception ex)
            {
                TempData["msg"] = " Something went wrong!!!";
            }
            return RedirectToAction("CreatePrescriptions", "UploadPrescription");
        }
    }
}
