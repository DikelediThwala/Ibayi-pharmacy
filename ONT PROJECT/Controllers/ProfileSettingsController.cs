using Microsoft.AspNetCore.Mvc;
using ONT_PROJECT.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ONT_PROJECT.Controllers
{
    public class ProfileSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProfileSettingsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // Show list of all customers (report)
        public IActionResult Index()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null) return RedirectToAction("Login", "CustomerRegister");

            var user = _context.TblUsers.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null) return RedirectToAction("Login", "CustomerRegister");

            var user = _context.TblUsers.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(TblUser model)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null) return RedirectToAction("Login", "CustomerRegister");

            var user = _context.TblUsers.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Idnumber = model.Idnumber;
            user.PhoneNumber = model.PhoneNumber;
            user.Allergies = model.Allergies;

            _context.SaveChanges();

            // ✅ Save success message in TempData
            TempData["SuccessMessage"] = "Your profile was updated successfully.";

            return RedirectToAction("Index");
        }
    }
}
