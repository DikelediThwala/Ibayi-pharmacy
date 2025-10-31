using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ONT_PROJECT.Controllers
{
    public class CustomerRegisterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public CustomerRegisterController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
            return RedirectToAction("Success"); // Stay on same page
        }

        public IActionResult Success()
        {
            return View();
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




        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Please enter your email.";
                return View();
            }

            var user = _context.TblUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user == null)
            {
                // Do not reveal that the email doesn't exist (security best practice)
                ViewBag.Message = "If an account with that email exists, a reset link has been sent.";
                return View();
            }

            // Generate secure reset token
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            user.ResetToken = token;
            user.TokenExpiry = DateTime.UtcNow.AddHours(1);

            _context.SaveChanges();

            // Build reset link
            var resetLink = Url.Action("UserResetPassword", "CustomerRegister",
                new { token = token, email = user.Email }, Request.Scheme);

            // Send email
            SendPasswordResetEmail(user.Email, resetLink);

            ViewBag.Message = "If an account with that email exists, a reset link has been sent.";
            return View();
        }

        private void SendPasswordResetEmail(string email, string resetLink)
        {
            var smtpHost = _configuration["SmtpSettings:Host"];
            var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
            var smtpUser = _configuration["SmtpSettings:User"];
            var smtpPass = _configuration["SmtpSettings:Password"];
            var fromAddress = new MailAddress(smtpUser, "ONT App");

            var toAddress = new MailAddress(email);
            const string subject = "GRP-04-04:Password Reset Request";
            string body = $@"
        <p>Hello,</p>
        <p>Click the link below to reset your password:</p>
        <p><a href='{resetLink}'>Reset Password</a></p>
        <p>This link will expire in 1 hour.</p>";

            using (var smtp = new SmtpClient())
            {
                smtp.Host = smtpHost;
                smtp.Port = smtpPort;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                smtp.Timeout = 20000;

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(message);
                }
            }
        }


        [HttpGet]
        public IActionResult UserResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return BadRequest("Invalid password reset request.");
            }

            var user = _context.TblUsers.FirstOrDefault(u => u.Email == email && u.ResetToken == token && u.TokenExpiry > DateTime.UtcNow);
            if (user == null)
            {
                return BadRequest("Invalid or expired token.");
            }

            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserResetPassword(string email, string token, string newPassword, string confirmPassword)
        {
            var user = _context.TblUsers.FirstOrDefault(u => u.Email == email && u.ResetToken == token && u.TokenExpiry > DateTime.UtcNow);
            if (user == null)
            {
                ViewBag.Error = "Invalid or expired token.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                ViewBag.Email = email;
                ViewBag.Token = token;
                return View();
            }

            user.Password = newPassword; // You can hash it later with BCrypt or SHA256
            user.ResetToken = null;
            user.TokenExpiry = null;
            _context.SaveChanges();

            ViewBag.Success = "Password reset successful. You can now log in.";
            return RedirectToAction("Register", "CustomerRegister");
        }

        private string HashPassword(string password)
        {
            return password;
        }

        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            return enteredPassword == storedPassword;
        }
    }
}
