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

      
        // POST: /CustomerRegister/Register
        // GET: Register
        public IActionResult Register()
        {
            var activeIngredients = _context.ActiveIngredient
                .Select(ai => new SelectListItem
                {
                    Value = ai.ActiveIngredientId.ToString(),
                    Text = ai.Ingredients
                }).ToList();

            ViewBag.ActiveIngredients = activeIngredients;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(TblUser model, string Password, string ConfirmPassword, List<int> SelectedAllergyIds)
        {
            // Validate passwords
            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                LoadAllergyDropdown();
                return View(model);
            }

            // Check if email already exists (case-insensitive)
            if (_context.TblUsers.Any(u => u.Email.ToLower() == model.Email.ToLower()))
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                LoadAllergyDropdown();
                return View(model);
            }

            // Prepare user
            model.Password = HashPassword(Password);
            model.Role = "Customer";

            // Prepare customer linked to the same user object
            var customer = new Customer
            {
                CustomerNavigation = model // directly link — no duplicate user
            };

            // Add allergies to customer
            foreach (var allergyId in SelectedAllergyIds)
            {
                customer.CustomerAllergies.Add(new CustomerAllergy
                {
                    ActiveIngredientId = allergyId
                });
            }

            // Add customer (this also adds the user, because of navigation property)
            _context.Customers.Add(customer);

            // Save everything at once
            _context.SaveChanges();

            TempData["Success"] = "Registration successful. Please login.";
            return RedirectToAction("Login");
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
