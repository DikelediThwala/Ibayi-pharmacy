using Microsoft.AspNetCore.Mvc;
using ONT_PROJECT.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.Build.Framework;

namespace ONT_PROJECT.Controllers
{
    public class ManagerSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ManagerSettingsController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Personal Details
        public IActionResult Details()
        {
            var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Home");

            var user = _context.TblUsers.Find(userId);
            if (user == null)
                return NotFound();

            var model = new UserSettingsViewModel
            {
                Title = user.Title,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            ViewData["ActiveTab"] = "details";
            return View(model);
        }
        [HttpPost]
        public IActionResult UpdateDetails(UserSettingsViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Details", model);

            var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");
            var user = _context.TblUsers.Find(userId);
            if (user == null)
                return NotFound();

            user.Title = model.Title;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;

            // Handle profile picture upload
            if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    model.ProfilePicture.CopyTo(ms);
                    user.ProfilePicture = ms.ToArray();
                }
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Details updated successfully!";
            return RedirectToAction("Details");
        }


        // Change Password
        public IActionResult Password()
        {
            ViewData["ActiveTab"] = "password";
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(UserSettingsViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Password", model);

            var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");
            var user = _context.TblUsers.Find(userId);
            if (user == null)
                return NotFound();

            if (user.Password != model.CurrentPassword) // hash check if applicable
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                return View("Password", model);
            }

            user.Password = model.NewPassword; // hash if needed
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction("Password");
        }
    }
}
