using Microsoft.AspNetCore.Mvc;

namespace ONT_PROJECT.Controllers
{
    public class PharmacistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> CreateUser()
        {
            return View();
        }
        public async Task<IActionResult> CreateDoctor()
        {
            return View();
        }
        public async Task<IActionResult> LoadPrescription()
        {
            return View();
        }
        public async Task<IActionResult> ViewPrescription()
        {
            return View();
        }
        public async Task<IActionResult> DispensePrescription()
        {
            return View();
        }
    }
}
