//using Microsoft.AspNetCore.Mvc;

//namespace ONT_PROJECT.Controllers
//{
//    public class CustomerController : Controller
//    {
//        public IActionResult Dashboard()
//        {
//            int? userId = HttpContext.Session.GetInt32("UserId");
//            if (!userId.HasValue)
//            {
//                return RedirectToAction("Login", "CustomerRegister");
//            }

//            var user = _context.TblUsers.Find(userId.Value);
//            return View(user);  // Pass user to the view or use ViewBag/ViewData
//        }

//        public IActionResult Index()
//        {
//            return View();
//        }
//        public IActionResult Create()
//        {
//            return View();
//        }
//    }
//}
using Microsoft.AspNetCore.Mvc;
using ONT_PROJECT.Models;  // Add this to access ApplicationDbContext and models

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
                return RedirectToAction("Login", "CustomerRegister");
            }

            var user = _context.TblUsers.Find(userId.Value);
            return View(user);  // Pass user to the view or use ViewBag/ViewData
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


