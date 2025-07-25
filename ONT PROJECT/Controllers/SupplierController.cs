using Microsoft.AspNetCore.Mvc;
using ONT_PROJECT.Models;
using Microsoft.EntityFrameworkCore; 


namespace ONT_PROJECT.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupplierController(ApplicationDbContext context)
        {
            _context = context;
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
