using Microsoft.AspNetCore.Mvc;
using IBayiLibrary.Repository;
using IBayiLibrary.Models.Domain;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ONT_PROJECT.Controllers
{
    public class PharmacistController : Controller
    {

        private readonly IUserRepository _personRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IOrderRepository _orderRepository;

        public PharmacistController(IUserRepository personRepository, IPrescriptionRepository prescriptionRepository,IOrderRepository orderRepository)
        {
            _personRepository = personRepository;
            _prescriptionRepository = prescriptionRepository;
            _orderRepository = orderRepository;

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
        public async Task<IActionResult> GetOrdersMedication()
        {
            var results = await _orderRepository.GetAllOrders() ?? new List<tblOrder>();

            var top5 = results
                .OrderByDescending(o => o.DatePlaced)
                .Take(5);

            return View(top5);
        }


        public async Task<IActionResult> Index()
        {
            var totalOrders = await _orderRepository.TotalNumberOfOrders();
            ViewBag.TotalOrders = totalOrders;
            return View();
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
                //if (!ModelState.IsValid)
                //    return View(user);
                var newUser = user;
                var role = user;
                // Auto-generate password
                string generatedPassword = PasswordGenerator.GeneratePassword();
                newUser.Password = generatedPassword;
                role.Role = "Customer";
                bool addPerson = await _personRepository.AddAsync(user);
                if (addPerson)
                {
                    TempData["msg"] = "Sucessfully Added";
                }
                else
                {
                    TempData["msg"] = "Could not add";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = " Something went wrong!!!";
            }
            return RedirectToAction("Index");          
        }      
        
    }
}
