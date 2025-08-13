using Microsoft.AspNetCore.Mvc;
using ONT_PROJECT.Models;  

namespace ONT_PROJECT.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        // ✅ Inject ApplicationDbContext
        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Dashboard", "Customer");
            }

            var user = _context.TblUsers.Find(userId.Value);
            return View(user); 
        }

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


