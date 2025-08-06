using Microsoft.AspNetCore.Mvc;

namespace ONT_PROJECT.Controllers
{
    public class CustomerOrderController : Controller
    {
        public IActionResult Order()
        {
            return View();
        }
        public IActionResult OrderedMedication()
        {
            return View();
        }
    }
}
