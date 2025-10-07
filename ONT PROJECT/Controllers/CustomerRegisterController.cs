using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;
using System.Linq;
using System.Security.Claims;
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
        // GET: Register
        public IActionResult Register()
        {
            LoadAllergyDropdown(); // Load allergies alphabetically
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
                CustomerNavigation = model // directly link
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

            // ===== Automatically log in the user =====
            HttpContext.Session.SetInt32("UserId", model.UserId); // UserId is generated after SaveChanges
            HttpContext.Session.SetString("UserEmail", model.Email);
            HttpContext.Session.SetString("UserFirstName", model.FirstName);
            HttpContext.Session.SetString("UserLastName", model.LastName); 
            HttpContext.Session.SetString("UserRole", model.Role);

            return RedirectToAction("Dashboard", "Customer");
        }

        // Updated LoadAllergyDropdown method
        private void LoadAllergyDropdown(List<int>? selectedIds = null)
        {
            selectedIds ??= new List<int>();

            ViewBag.ActiveIngredients = _context.ActiveIngredient
                .Select(ai => new SelectListItem
                {
                    Value = ai.ActiveIngredientId.ToString(),
                    Text = ai.Ingredients,
                    Selected = selectedIds.Contains(ai.ActiveIngredientId) // Pre-select if in list
                })
                .OrderBy(ai => ai.Text) // Alphabetical order
                .ToList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var user = _context.TblUsers.FirstOrDefault(u => u.Email == Email);

            if (user != null && VerifyPassword(Password, user.Password))
            {
                // --- 1. Create claims for authentication ---
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // --- 2. Sign in with cookie ---
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                );

                // --- 3. Store extra data in session if needed ---
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

                // --- 4. Redirect based on role ---
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

            ViewData["LoginError"] = "Username and email are invalid.";
            return View("~/Views/User/Login.cshtml", new TblUser { Email = Email });

        }




        private string HashPassword(string password)
        {
            return password;
        }

        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            // TODO: replace with proper verification
            return enteredPassword == storedPassword;
        }
    }
}
