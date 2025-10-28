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
        //[ValidateAntiFogeryToken]
        public async Task<IActionResult> CreateUser(tblUser user)
        {
            try
            {
                
                var newUser = user;
                var role = user;
                // Auto-generate password
                string generatedPassword = PasswordGenerator.GeneratePassword();
                newUser.Password = generatedPassword;
                role.Role = "Customer";

                if (await _userRepository.CheckIDNumberExistsAsync(user.IDNumber))
                {
                    ModelState.AddModelError("IDNumber", "This ID Number is already registered.");
                    return View(user);
                }
                var person = await _personRepository.AddAsync(user);             
                if (!string.IsNullOrEmpty(user.Email))
                {
                    string emailBody = $@"
                        <p>Hello {user.FirstName}<br>{user.LastName}</p>                       
                        <p>Your Order is ready for collection</p>
                        <p>Here's your passowrd {user.Password}</p> ";
                        


                    _emailService.Send(user.Email,"", emailBody);
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = " Something went wrong!!!";
            }
            return RedirectToAction("GetAllCustomers");          
        }
        [HttpGet]
        public JsonResult CheckEmailExists(string email)
        {
            bool exists = _context.TblUsers.Any(u => u.Email == email);
            return Json(new { exists });
        }
       
    }
}
