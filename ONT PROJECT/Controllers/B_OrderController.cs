using Microsoft.AspNetCore.Mvc;

namespace ONT_PROJECT.Controllers
{
    public class B_OrderController : Controller
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
