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

        // Add this constructor for dependency injection
        public ProfileSettingsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult UpdateProfile()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null) return RedirectToAction("Login", "CustomerRegister");

            var user = _context.TblUsers.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(TblUser model, IFormFile ProfilePicture)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.TblUsers.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
                return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Idnumber = model.Idnumber;
            user.PhoneNumber = model.PhoneNumber;
            user.Allergies = model.Allergies;

            if (ProfilePicture != null && ProfilePicture.Length > 0)
            {
                var fileName = Path.GetFileName(ProfilePicture.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfilePicture.CopyToAsync(stream);
                }

                // Uncomment and set path if you want to save it
                // user.ProfilePicturePath = "/uploads/" + fileName;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("UpdateProfile");
        }
    }
}
