using Microsoft.AspNetCore.Mvc;

namespace ONT_PROJECT.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}

