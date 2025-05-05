using Microsoft.AspNetCore.Mvc;

namespace ONT_PROJECT.Controllers
{
    public class Prescription : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult create()
        {
            return View();
        }
    }
}
