using Microsoft.AspNetCore.Mvc;

namespace ONT_PROJECT.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }

    }
}
