using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace ONT_PROJECT.Controllers
{
    public class CustomerRegisterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerRegisterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /CustomerRegister/Register
        [HttpGet]
        public IActionResult Register()
        {
            LoadAllergyDropdown();
            return View();
        }

        // POST: /CustomerRegister/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(TblUser model, string Password, string ConfirmPassword)
        {
            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
            }

            if (_context.TblUsers.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email is already registered.");
            }

            if (ModelState.IsValid)
            {
                model.Password = HashPassword(Password);
                _context.TblUsers.Add(model);
                _context.SaveChanges();

                TempData["Success"] = "Account created successfully. Please log in.";
                LoadAllergyDropdown();
                return View("Register"); // reload same combined view
            }

            LoadAllergyDropdown();
            return View("Register", model);
        }

        // POST: /CustomerRegister/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string Email, string Password)
        {
            var user = _context.TblUsers.FirstOrDefault(u => u.Email == Email);

            if (user != null && VerifyPassword(Password, user.Password))
            {
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserFirstName", user.FirstName);
                HttpContext.Session.SetString("UserRole", user.Role);

                TempData["Success"] = "Login successful!";

                switch (user.Role)
                {
                    case "Customer":
                        return RedirectToAction("Dashboard", "Customer");
                    case "Pharmacist":
                        return RedirectToAction("Index", "Pharmacist");
                    case "PharmacyManager":
                        return RedirectToAction("Dashboard", "Manager");
                    default:
                        return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Invalid login credentials.");
            LoadAllergyDropdown();
            return View("Register", new TblUser { Email = Email });
        }

        private void LoadAllergyDropdown()
        {
            ViewBag.ActiveIngredients = _context.ActiveIngredient
                .Select(ai => new SelectListItem
                {
                    Value = ai.ActiveIngredientId.ToString(),
                    Text = ai.Ingredients
                }).ToList();
        }

        private string HashPassword(string password)
        {
            // TODO: replace with proper hashing (e.g., BCrypt)
            return password;
        }

        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            // TODO: replace with proper verification
            return enteredPassword == storedPassword;
        }
    }
}
