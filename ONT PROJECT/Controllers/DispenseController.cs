using Microsoft.AspNetCore.Mvc;

namespace ONT_PROJECT.Controllers
{
    public class DispenseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
