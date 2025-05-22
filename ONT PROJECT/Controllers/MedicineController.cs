using Microsoft.AspNetCore.Mvc;
using ONT_PROJECT.Models;

namespace ONT_PROJECT.Controllers
{
    public class MedicineController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        // GET: Medicines/Create
        public IActionResult Create()
        {
            var model = new Medicine();

            // Make sure Ingredients list is initialized to avoid null refs
            model.Ingredients = new List<string>();

            return View(model);
        }

    }
}
