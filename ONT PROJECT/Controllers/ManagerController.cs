using Microsoft.AspNetCore.Mvc;

namespace ONT_PROJECT.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Report()
        {
            return View();
        }
    }
}
