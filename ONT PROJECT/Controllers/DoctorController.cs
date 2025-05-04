using Microsoft.AspNetCore.Mvc;

namespace ONT_PROJECT.Controllers
{
    public class DoctorController : Controller
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
