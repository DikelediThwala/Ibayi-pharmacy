using Microsoft.AspNetCore.Mvc;

namespace ONT_PROJECT.Controllers
{
    public class PharmacistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
      
        public IActionResult CreateUser()
        {
            return View();
        }
        public IActionResult CreateDoctor()
        {
            return View();
        }
        public IActionResult LoadPrescription()
        {
            return View();
        }
       
        public IActionResult ViewPrescription()
        {
            return View();
        }
        public IActionResult DispensePrescription()
        {
            return View();
        }
    }
}
