using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            // existing validation...

            // Save user
            model.Password = HashPassword(Password);
            model.Role = "Customer";
            _context.TblUsers.Add(model);
            _context.SaveChanges();

            // Save customer
            var customer = new Customer
            {
                CustomerId = model.UserId,
                CustomerNavigation = model
            };
            _context.Customers.Add(customer);
            _context.SaveChanges();

            // Save allergies
            foreach (var allergyId in SelectedAllergyIds)
            {
                var customerAllergy = new CustomerAllergy
                {
                    CustomerId = customer.CustomerId,
                    ActiveIngredientId = allergyId
                };
                _context.CustomerAllergies.Add(customerAllergy);
            }
            _context.SaveChanges();

            TempData["Success"] = "Registration successful. Please login.";
            return RedirectToAction("Dashboard", "Customer");
        }

        
        public IActionResult Login()
        {
            return View();
        }

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
                return RedirectToAction("Dashboard", "Customer"); 
            }

            ModelState.AddModelError("", "Invalid login credentials");
            return View();
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == storedHash;
        }
    }
}