using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using ONT_PROJECT.Models;
using Org.BouncyCastle.Crypto.Generators;
using System.Linq;
using System.Text.RegularExpressions;

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
                UserId = user.UserId,  // <--- ADD THIS
                Title = user.Title?.Trim(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ExistingProfilePicture = user.ProfilePicture
            };

            // Create SelectList with selected value
            ViewBag.TitleList = new SelectList(
                new List<string> { "Mr", "Mrs", "Miss", "Dr" },
                model.Title // must match exactly one of the items
            );

            ViewData["ActiveTab"] = "details";
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateDetails(UserSettingsViewModel model)
        {
            // Step 0: Debug ModelState
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
                }
            }

            var user = _context.TblUsers.FirstOrDefault(u => u.UserId == model.UserId);

            // If user not found
            if (user == null)
            {
                ModelState.AddModelError("", $"No user found with ID {model.UserId}. Please check the database.");
                ViewBag.TitleList = new SelectList(new List<string> { "Mr", "Mrs", "Miss", "Dr" }, model.Title?.Trim());
                model.ExistingProfilePicture = null;
                return View("Details", model);
            }

            // Always update user if ModelState is valid
            if (!ModelState.IsValid)
            {
                model.ExistingProfilePicture = user.ProfilePicture;
                ViewBag.TitleList = new SelectList(new List<string> { "Mr", "Mrs", "Miss", "Dr" }, model.Title?.Trim());
                return View("Details", model);
            }

            try
            {
                // Update basic details
                user.Title = model.Title?.Trim();
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;

                // Update profile picture only if a new one is uploaded
                if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
                {
                    using var ms = new MemoryStream();
                    model.ProfilePicture.CopyTo(ms);
                    user.ProfilePicture = ms.ToArray();
                }

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Details updated successfully!";
                return RedirectToAction("Details");
            }
            catch (Exception ex)
            {
                model.ExistingProfilePicture = user.ProfilePicture;
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.TitleList = new SelectList(new List<string> { "Mr", "Mrs", "Miss", "Dr" }, model.Title?.Trim());
                return View("Details", model);
            }
        }

        public IActionResult Password()
        {
            ViewData["ActiveTab"] = "password";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(UserSettingsViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Password", model);

            var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized();

            var user = _context.TblUsers.Find(userId);
            if (user == null)
                return NotFound();

            if (user.Password != model.CurrentPassword)
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                return View("Password", model);
            }

            user.Password = model.NewPassword;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction("Password");
        }


    }
}
