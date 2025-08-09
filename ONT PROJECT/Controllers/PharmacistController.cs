using Microsoft.AspNetCore.Mvc;
using IBayiLibrary.Repository;
using IBayiLibrary.Models.Domain;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ONT_PROJECT.Controllers
{
    public class PharmacistController : Controller
    {

        //This method is responsible for Auto generating passsword for the customers
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


        private readonly IUserRepository _personRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;
        
        public PharmacistController(IUserRepository personRepository,IPrescriptionRepository prescriptionRepository)
        {
            _personRepository = personRepository;
            _prescriptionRepository = prescriptionRepository;
            
        }




        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateUser()
        {

            return View();
        }
      
        
        public IActionResult ProcessOrder()
        {

            return View();
        }
        public IActionResult CreateDoctor()
        {
            return View();
        }
        public IActionResult OrderDetails()
        {

            return View();
        }
        public IActionResult LoadPrescription()
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
        public async Task<IActionResult> ViewPrescription()
        {
            return View();
        }
        public IActionResult DispensePrescription()
        {
            return View();
        }
        public IActionResult UnprocessedPrescription()
        {
            return View();
        }
    }
}
