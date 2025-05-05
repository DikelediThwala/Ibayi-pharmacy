using Microsoft.AspNetCore.Mvc;

namespace ONT_PROJECT.Controllers
{
    public class ClientController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}

