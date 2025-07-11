using Microsoft.AspNetCore.Mvc;
using ONT_PROJECT.Models;
using System.Security.Cryptography;
using System.Text;

namespace ONT_PROJECT.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
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

            user.Password = GenerateRandomPassword(10);
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
            TempData["GeneratedPassword"] = user.Password;

            return RedirectToAction("Index");
        }

    }
}


















//using Microsoft.AspNetCore.Mvc;
//using ONT_PROJECT.Models;
//using System.Security.Cryptography;
//using System.Text;

//namespace ONT_PROJECT.Controllers
//{
//    public class UserController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public UserController(ApplicationDbContext context)
//        {
//            _context = context;
//        }
//        public IActionResult Index()
//        {
//            var managers = _context.TblUsers.Where(u => u.Role == "Manager").ToList();
//            var pharmacists = _context.TblUsers.Where(u => u.Role == "Pharmacist").ToList();
//            var customers = _context.TblUsers.Where(u => u.Role == "Customer").ToList();

//            ViewBag.Managers = managers;
//            ViewBag.Pharmacists = pharmacists;
//            ViewBag.Customers = customers;

//            return View();
//        }

//        public IActionResult Create()
//        {
//            return View();
//        }
//        public IActionResult Login()
//        {
//            return View();
//        }

//        public IActionResult Register()
//        {
//            return View();
//        }


//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult SaveUser(TblUser user)
//        {
//            if (ModelState.IsValid)
//            {
//                user.Password = GenerateRandomPassword(10);
//                _context.TblUsers.Add(user);
//                _context.SaveChanges();

//                int newUserId = user.UserId;

//                if (user.Role == "Customer")
//                {
//                    var customer = new Customer { CustomerId = newUserId };
//                    _context.Customers.Add(customer);
//                }
//                else if (user.Role == "Pharmacist")
//                {
//                    var pharmacist = new Pharmacist { PharmacistId = newUserId };
//                    _context.Pharmacists.Add(pharmacist);
//                }
//                else if (user.Role == "PharmacyManager")
//                {
//                    var manager = new PharmacyManager { PharmacyManagerId = newUserId };
//                    _context.PharmacyManagers.Add(manager);
//                }

//                _context.SaveChanges();

//                TempData["GeneratedPassword"] = user.Password;

//                return RedirectToAction("Index");
//            }

//            return View("Create", user);
//        }



//        private string GenerateRandomPassword(int length)
//        {
//            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$!";
//            var password = new StringBuilder();
//            using (var rng = RandomNumberGenerator.Create())
//            {
//                byte[] uintBuffer = new byte[sizeof(uint)];

//                while (password.Length < length)
//                {
//                    rng.GetBytes(uintBuffer);
//                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
//                    var index = num % (uint)validChars.Length;
//                    password.Append(validChars[(int)index]);
//                }
//            }

//            return password.ToString();
//        }
//    }
//}
