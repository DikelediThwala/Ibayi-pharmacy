using Microsoft.AspNetCore.Mvc;

namespace ONT_PROJECT.Controllers
{
    public class CustomerReport : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
