using Microsoft.AspNetCore.Mvc;

namespace ONT_PROJECT.Controllers
{
    public class ActiveIngredientController : Controller
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
