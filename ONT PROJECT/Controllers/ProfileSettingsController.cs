using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ONT_PROJECT.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ONT_PROJECT.Controllers
{
    public class ProfileSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProfileSettingsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // Show list of all customers (report)
        [HttpGet]
        public IActionResult Index(bool? edit)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null) return RedirectToAction("Login", "CustomerRegister");

            var user = _context.TblUsers.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerNavigation.UserId == user.UserId);
            var selectedAllergies = new List<int>();
            if (customer != null)
            {
                selectedAllergies = _context.CustomerAllergies
                    .Where(ca => ca.CustomerId == customer.CustomerId)
                    .Select(ca => ca.ActiveIngredientId)
                    .ToList();
            }

            var allAllergies = _context.ActiveIngredient
                .Select(ai => new SelectListItem
                {
                    Value = ai.ActiveIngredientId.ToString(),
                    Text = ai.Ingredients
                })
                .OrderBy(a => a.Text)
                .ToList();

            ViewBag.SelectedAllergies = selectedAllergies;
            ViewBag.ActiveIngredients = allAllergies;

            // Toggle edit mode
            ViewBag.EditMode = edit ?? false;

            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(TblUser model, List<int> SelectedAllergyIds)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null) return RedirectToAction("Login", "CustomerRegister");

            var user = _context.TblUsers.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();

            // Update user fields
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Idnumber = model.Idnumber;
            user.PhoneNumber = model.PhoneNumber;

            // Update allergies
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerNavigation.UserId == user.UserId);
            if (customer != null)
            {
                // Remove all existing allergies
                var existingAllergies = _context.CustomerAllergies
                    .Where(ca => ca.CustomerId == customer.CustomerId)
                    .ToList();
                _context.CustomerAllergies.RemoveRange(existingAllergies);

                // Add selected allergies
                if (SelectedAllergyIds != null)
                {
                    foreach (var allergyId in SelectedAllergyIds)
                    {
                        _context.CustomerAllergies.Add(new CustomerAllergy
                        {
                            CustomerId = customer.CustomerId,    // important!
                            ActiveIngredientId = allergyId
                        });
                    }
                }
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Your profile was updated successfully.";

            return RedirectToAction("Index");
        }


        //[HttpGet]
        //public IActionResult Edit()
        //{
        //    var email = HttpContext.Session.GetString("UserEmail");
        //    if (email == null) return RedirectToAction("Login", "CustomerRegister");

        //    var user = _context.TblUsers.FirstOrDefault(u => u.Email == email);
        //    if (user == null) return NotFound();

        //    return View(user);
        //}

        //[HttpPost]
        //public IActionResult Edit(TblUser model)
        //{
        //    var email = HttpContext.Session.GetString("UserEmail");
        //    if (email == null) return RedirectToAction("Login", "CustomerRegister");

        //    var user = _context.TblUsers.FirstOrDefault(u => u.Email == email);
        //    if (user == null) return NotFound();

        //    user.FirstName = model.FirstName;
        //    user.LastName = model.LastName;
        //    user.Idnumber = model.Idnumber;
        //    user.PhoneNumber = model.PhoneNumber;
        //    user.Allergies = model.Allergies;

        //    _context.SaveChanges();

        //    // ✅ Save success message in TempData
        //    TempData["SuccessMessage"] = "Your profile was updated successfully.";

        //    return RedirectToAction("Index");
        //}
    }
}
