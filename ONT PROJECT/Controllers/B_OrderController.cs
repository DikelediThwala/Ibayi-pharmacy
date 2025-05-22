using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;

namespace ONT_PROJECT.Controllers
{
    public class B_OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public B_OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        //public async Task<IActionResult> Index()
        //{
        //    var orders = await _context.B_Orders
        //        .Include(o => o.Supplier)
        //        .Include(o => o.OrderLines)
        //            .ThenInclude(ol => ol.Medication)
        //        .ToListAsync();

        //    return View(orders);
        //}


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            
            return View();
        }
        public IActionResult OutOfStock()
        {
            return View();
        }
        public IActionResult Selected()
        {
            return View();
        }

    }
}
