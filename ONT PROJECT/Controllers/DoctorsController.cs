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
        [HttpGet]
        public IActionResult CreateDoctor()
        {
            
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDoctor(Doctor doctor, string returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["ReturnUrl"] = returnUrl; // keep returnUrl for the view
                    return View(doctor);
                }

                // Check for existing email
                bool emailExists = await _doctorRepository.CheckEmailExistsAsync(doctor.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "This email address is already registered.");
                    TempData["msg"] = "Email already exists!";
                    ViewData["ReturnUrl"] = returnUrl;
                    return View(doctor);
                }

                // Add doctor
                bool addPerson = await _doctorRepository.AddAsync(doctor);
                TempData["msg"] = addPerson ? "Successfully Added" : "Could not add";

                // ✅ Redirect to previous page if returnUrl exists and is local
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // ✅ Otherwise, fallback to your desired default page
                return RedirectToAction("CreatePrescForWalkins", "UploadPrescription");
            }
            catch (Exception)
            {
                TempData["msg"] = "Something went wrong!!!";
                ViewData["ReturnUrl"] = returnUrl;
                return View(doctor);
            }
        }
    }
}

