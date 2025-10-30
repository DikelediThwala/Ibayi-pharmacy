using IBayiLibrary.Models.Domain;
using IBayiLibrary.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ONT_PROJECT.Models;
using System;

namespace ONT_PROJECT.Controllers
{
    public class PharmacistController : Controller
    {

        private readonly IUserRepository _personRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnproccessedPrescriptionRepository _unproccessedPrescriptionRepository;
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        
        public PharmacistController(IUserRepository personRepository, IPrescriptionRepository prescriptionRepository,IOrderRepository orderRepository, IUnproccessedPrescriptionRepository unproccessedPrescriptionRepository, IUserRepository userRepository,ApplicationDbContext context,EmailService emailService)
        {
            _personRepository = personRepository;
            _prescriptionRepository = prescriptionRepository;
            _orderRepository = orderRepository;
            _unproccessedPrescriptionRepository = unproccessedPrescriptionRepository;
            _userRepository = userRepository;
            _context = context;
            _emailService = emailService;
        }
        public static class PasswordGenerator
        {
            private static Random random = new Random();

            public static string GeneratePassword(int length = 10)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
                return new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }
        }
        public async Task<IActionResult> GetAllCustomers()
        {
            var results = await _userRepository.GetCustomers();
            return View(results);
        }
        public async Task<IActionResult> Index()
        {
            // --- Dashboard Stats ---
            var totalOrders = await _orderRepository.TotalNumberOfOrders();
            ViewBag.TotalOrders = totalOrders;

            var unprocPresc = await _unproccessedPrescriptionRepository.NumberOfUnprocessedPresc();
            ViewBag.NumberOfUnprocessedPresc = unprocPresc;

            var readyOrders = await _userRepository.NoOfCustomer();
            ViewBag.NoOfReadyOrders = readyOrders;

            // --- Logged-in User Info ---
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(userId.Value);

                // if user found, build full name
                if (user != null)
                {
                    ViewBag.FullName = $"{user.FirstName} {user.LastName}";
                }
                else
                {
                    ViewBag.FullName = "Guest";
                }
            }
            else
            {
                ViewBag.FullName = "Guest";
            }

            // --- Recent Orders ---
            var results = await _orderRepository.GetAllOrders() ?? new List<tblOrder>();
            var top5 = results
                .OrderByDescending(o => o.DatePlaced)
                .Take(5);

            return View(top5);
        }
        public IActionResult CreateUser()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(tblUser user, string returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["ReturnUrl"] = returnUrl; // keep returnUrl for the view
                    return View(user);
                }

                // Auto-generate password and set role
                user.Password = PasswordGenerator.GeneratePassword();
                user.Role = "Customer";

                if (await _userRepository.CheckIDNumberExistsAsync(user.IDNumber))
                {
                    ModelState.AddModelError("IDNumber", "This ID Number is already registered.");
                    ViewData["ReturnUrl"] = returnUrl;
                    return View(user);
                }

                var person = await _personRepository.AddAsync(user);

                if (!string.IsNullOrEmpty(user.Email))
                {
                    var resetLink = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/User/ResetPassword?email={user.Email}";
                    string emailBody = $@"
                <p>Hello {user.FirstName} {user.LastName}</p>                       
                <p>Here's your Temporary Password:</p>
                <strong>{user.Password}</strong> 
                <p>Please reset your password by clicking the link below:</p>
                <p><a href='{resetLink}'>Reset Password</a></p>";
                    _emailService.Send(user.Email, "GRP-04-04: Temporary Password", emailBody);
                }

                // Redirect back to returnUrl if provided, else fallback
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("GetAllCustomers");
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something went wrong!!!";
                ViewData["ReturnUrl"] = returnUrl;
                return View(user);
            }
        }

        [HttpGet]
        public JsonResult CheckEmailExists(string email)
        {
            bool exists = _context.TblUsers.Any(u => u.Email == email);
            return Json(new { exists });
        }
       
    }
}
