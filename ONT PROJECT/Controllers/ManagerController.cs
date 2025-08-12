using Microsoft.AspNetCore.Mvc;
using ONT_PROJECT.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ONT_PROJECT.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagerController(ApplicationDbContext context)
        {
            _context = context;
           
        }
        public IActionResult Dashboard()
        {
            var recentOrders = _context.BOrders
                .Include(o => o.BOrderLines)
                    .ThenInclude(ol => ol.Medicine)
                .OrderByDescending(o => o.DatePlaced)
                .Take(5)
                .ToList();

            return View(recentOrders);
        }
        public IActionResult Report()
        {
            return View();
        }
    }
}
