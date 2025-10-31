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
        public async Task<IActionResult> CreateDoctor(Doctor doctor, string returnUrl = null)
        {
            try
            {
               
                bool emailExists = await _doctorRepository.CheckEmailExistsAsync(doctor.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "This email address is already registered.");
                    TempData["msg"] = "Email already exists!";
                    ViewData["ReturnUrl"] = returnUrl;
                    return View(doctor);
                }

                bool added = await _doctorRepository.AddAsync(doctor);
                TempData["msgs"] = added ? " Doctor Successefully Added" : "Could not add";

                // Normalize returnUrl
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something went wrong!!! " + ex.Message;
                ViewData["ReturnUrl"] = returnUrl;
                return View(doctor);
            }
            return RedirectToAction("CreateDoctor");
        }


    }
}

