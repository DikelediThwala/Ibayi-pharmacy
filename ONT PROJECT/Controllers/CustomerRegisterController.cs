using Microsoft.AspNetCore.Mvc;
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
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(TblUser model, string Password, string ConfirmPassword)
        {
            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match");
                return View(model);
            }

            if (_context.TblUsers.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("", "Email already exists");
                return View(model);
            }

            model.Password = HashPassword(Password);
            model.Role = "Customer";

            // Add TblUser first to generate UserId
            _context.TblUsers.Add(model);
            _context.SaveChanges(); // Save to get UserId

            // Create Customer with CustomerId set to the generated UserId
            var customer = new Customer
            {
                CustomerId = model.UserId,        // **Important: Set FK explicitly here**
                CustomerNavigation = model        // Optional: set navigation property
            };

            _context.Customers.Add(customer);
            _context.SaveChanges(); // Save the Customer record

            TempData["Success"] = "Registration successful. Please login.";
            return RedirectToAction("Login");
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string Email, string Password)
        {
            var user = _context.TblUsers.SingleOrDefault(u => u.Email == Email);

            if (user != null && VerifyPassword(Password, user.Password))
            {
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserFirstName", user.FirstName);
                HttpContext.Session.SetString("UserRole", user.Role);

                TempData["Success"] = "Login successful!";
                return RedirectToAction("Dashboard", "Customer"); // Or your landing page
            }

            ModelState.AddModelError("", "Invalid login credentials");
            return View();
        }

        // Hash password using SHA256
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // Verify entered password matches stored hashed password
        private bool VerifyPassword(string password, string storedHash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == storedHash;
        }
    }
}
