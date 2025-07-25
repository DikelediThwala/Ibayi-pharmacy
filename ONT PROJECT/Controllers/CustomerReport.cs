using Microsoft.AspNetCore.Mvc;
using ONT_PROJECT.Models;

namespace ONT_PROJECT.Controllers
{
    public class CustomerReport : Controller
    {
        public IActionResult Index()
        {
            var user = new TblUser
            {
                FirstName = "John",
                LastName = "Doe",
                Idnumber = "1234567890123",
                PhoneNumber = "0123456789",
                Email = "john@example.com",
                Allergies = "Penicillin,Peanuts"
            };

            return View(user);
        }
    }
}
