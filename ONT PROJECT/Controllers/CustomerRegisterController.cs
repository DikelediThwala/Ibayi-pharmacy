using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        public IActionResult Register()
        {
            LoadAllergyDropdown(); // Load allergies alphabetically
            return View();
        }

        // POST: /CustomerRegister/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(TblUser model, string Password, string ConfirmPassword, List<int> SelectedAllergyIds)
        {
            // Validate passwords
            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                LoadAllergyDropdown(SelectedAllergyIds);
                return View(model);
            }

            // Check if email already exists (case-insensitive)
            if (_context.TblUsers.Any(u => u.Email.ToLower() == model.Email.ToLower()))
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                LoadAllergyDropdown(SelectedAllergyIds);
                return View(model);
            }

            // Prepare user
            model.Password = HashPassword(Password);
            model.Role = "Customer";

            // Prepare customer linked to the same user object
            var customer = new Customer
            {
                CustomerNavigation = model
            };

            // Add allergies to customer
            if (SelectedAllergyIds != null)
            {
                foreach (var allergyId in SelectedAllergyIds)
                {
                    customer.CustomerAllergies.Add(new CustomerAllergy
                    {
                        ActiveIngredientId = allergyId
                    });
                }
            }

            _context.Customers.Add(customer);
            _context.SaveChanges();

            // Show success message and redirect to login panel
            TempData["Success"] = "Successfully registered! You can now log in.";
            return RedirectToAction("Register"); // Stay on same page
        }

        // Remote email validation
        [AcceptVerbs("GET", "POST")]
        public IActionResult IsEmailAvailable(string email)
        {
            bool emailExists = _context.TblUsers.Any(u => u.Email.ToLower() == email.ToLower());
            if (emailExists)
            {
                return Json("An email already exists");
            }
            return Json(true);
        }

        // Load allergy dropdown
        private void LoadAllergyDropdown(List<int>? selectedIds = null)
        {
            selectedIds ??= new List<int>();

            ViewBag.ActiveIngredients = _context.ActiveIngredient
                .Select(ai => new SelectListItem
                {
                    Value = ai.ActiveIngredientId.ToString(),
                    Text = ai.Ingredients,
                    Selected = selectedIds.Contains(ai.ActiveIngredientId)
                })
                .OrderBy(ai => ai.Text)
                .ToList();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var user = _context.TblUsers.FirstOrDefault(u => u.Email == Email);

            if (user != null && VerifyPassword(Password, user.Password))
            {
                // Create claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Sign in with cookie
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                );

                // Store session data
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserFirstName", user.FirstName);
                HttpContext.Session.SetString("UserLastName", user.LastName);
                HttpContext.Session.SetString("UserRole", user.Role);

                if (user.ProfilePicture != null && user.ProfilePicture.Length > 0)
                {
                    string base64String = Convert.ToBase64String(user.ProfilePicture);
                    HttpContext.Session.SetString("UserProfilePic", base64String);
                }
                else
                {
                    HttpContext.Session.SetString("UserProfilePic", "");
                }

                TempData["Success"] = "Login successful!";

                // Redirect based on role
                return user.Role switch
                {
                    "Customer" => RedirectToAction("Dashboard", "Customer"),
                    "Pharmacist" => RedirectToAction("Index", "Pharmacist"),
                    "PharmacyManager" => RedirectToAction("Dashboard", "Manager"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            ViewData["LoginError"] = "Username and password are invalid.";
            return View("~/Views/User/Login.cshtml", new TblUser { Email = Email });
        }

        // Placeholder for password hashing
        private string HashPassword(string password)
        {
            return password;
        }

        // Placeholder for password verification
        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            return enteredPassword == storedPassword;
        }
    }
}
