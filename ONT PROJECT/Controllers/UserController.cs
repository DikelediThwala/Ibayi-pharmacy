using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ONT_PROJECT.Models;
using System.Security.Cryptography;
using System.Text;

namespace ONT_PROJECT.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        public UserController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            var managers = _context.TblUsers.Where(u => u.Role == "PharmacyManager").ToList();
            var pharmacists = _context.TblUsers.Where(u => u.Role == "Pharmacist").ToList();
            var customers = _context.TblUsers.Where(u => u.Role == "Customer").ToList();

            ViewBag.Managers = managers;
            ViewBag.Pharmacists = pharmacists;
            ViewBag.Customers = customers;

            return View();
        }

        public IActionResult Create()
        {
            return View();
        }


        private string GenerateRandomPassword(int length)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$!";
            var password = new StringBuilder();
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (password.Length < length)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    var index = num % (uint)validChars.Length;
                    password.Append(validChars[(int)index]);
                }
            }

            return password.ToString();
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveUser(TblUser user)
        {
            if (!ModelState.IsValid)
            {
                
                var errorMessages = ModelState
                    .Where(kvp => kvp.Value.Errors.Count > 0)
                    .Select(kvp => $"{kvp.Key} is invalid or missing.")
                    .ToList();

                
                TempData["Error"] = string.Join("<br/>", errorMessages);
                return View("Create", user);
            }

            if (user.ProfileFile != null && user.ProfileFile.Length > 0)
            {
                using var ms = new MemoryStream();
                user.ProfileFile.CopyTo(ms);
                user.ProfilePicture = ms.ToArray();
            }

            var plainPassword = GenerateRandomPassword(10);
            user.Password = HashPassword(plainPassword);

            _context.TblUsers.Add(user);
            _context.SaveChanges();

            int newUserId = user.UserId;

            if (user.Role == "Customer")
                _context.Customers.Add(new Customer { CustomerId = newUserId });
            else if (user.Role == "Pharmacist")
            {
                var regNo = Request.Form["HealthCounsilRegNo"].ToString();

                var pharmacist = new Pharmacist
                {
                    PharmacistId = newUserId,
                    HealthCounsilRegNo = regNo
                };
                _context.Pharmacists.Add(pharmacist);
            }

            else if (user.Role == "PharmacyManager")
                _context.PharmacyManagers.Add(new PharmacyManager { PharmacyManagerId = newUserId });

            _context.SaveChanges();
            TempData["GeneratedPassword"] = plainPassword;


            var resetLink = Url.Action("ResetPassword", "User", new { email = user.Email }, protocol: Request.Scheme);

            string emailBody = $@" <p>Hello {user.FirstName},</p>
            <p>Your account has been created. Here is your temporary password:</p>
           <p><strong>{plainPassword}</strong></p>
            <p>Please reset your password by clicking the link below:</p>
            <p><a href='{resetLink}'>Reset Password</a></p>";

            _emailService.Send(user.Email, "Your Temporary Password", emailBody);

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var user = _context.TblUsers.FirstOrDefault(u => u.UserId == id);
            if (user == null)
                return NotFound();

            var titles = new List<string> { "Mr", "Mrs", "Miss", "Dr" };
            ViewBag.Titles = new SelectList(titles, user.Title);

            if (user.Role == "Pharmacist")
            {
                var pharmacist = _context.Pharmacists.FirstOrDefault(p => p.PharmacistId == id);
                if (pharmacist != null)
                    ViewBag.HealthCouncilRegNo = pharmacist.HealthCounsilRegNo;
            }

            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TblUser model)
        {

            if (ModelState.IsValid)
            {
                var existingUser = _context.TblUsers.FirstOrDefault(u => u.UserId == model.UserId);
                if (existingUser == null)
                    return NotFound();

                existingUser.FirstName = model.FirstName;
                existingUser.LastName = model.LastName;
                existingUser.Email = model.Email;
                existingUser.Idnumber = model.Idnumber;
                existingUser.PhoneNumber = model.PhoneNumber;
                existingUser.Title = model.Title;
                existingUser.Role = model.Role;

                if (model.ProfileFile != null && model.ProfileFile.Length > 0)
                {
                    using var ms = new MemoryStream();
                    model.ProfileFile.CopyTo(ms);
                    existingUser.ProfilePicture = ms.ToArray();

                }

                if (model.Role == "Pharmacist")
                {
                    string regNo = Request.Form["HealthCouncilRegNo"];
                    var pharmacist = _context.Pharmacists.FirstOrDefault(p => p.PharmacistId == model.UserId);
                    if (pharmacist != null)
                    {
                        pharmacist.HealthCounsilRegNo = regNo;
                    }
                }

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            var model = new ResetPasswordViewModel { Email = email };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.TblUsers.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
                return NotFound();

            user.Password = HashPassword(model.NewPassword);
            _context.SaveChanges();

            TempData["Message"] = "Password reset successful!";
            return RedirectToAction("Login", "Login"); 
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
    }
}
