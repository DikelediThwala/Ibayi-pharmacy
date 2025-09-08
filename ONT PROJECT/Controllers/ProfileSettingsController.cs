using Elfie.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

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

        [HttpGet]
        public IActionResult Settings()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null) return RedirectToAction("Login", "CustomerRegister");

            var user = _context.TblUsers.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();

            return View(user);
        }

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

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Idnumber = model.Idnumber;
            user.PhoneNumber = model.PhoneNumber;

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerNavigation.UserId == user.UserId);
            if (customer != null)
            {
                var existingAllergies = _context.CustomerAllergies
                    .Where(ca => ca.CustomerId == customer.CustomerId)
                    .ToList();
                _context.CustomerAllergies.RemoveRange(existingAllergies);

                if (SelectedAllergyIds != null)
                {
                    foreach (var allergyId in SelectedAllergyIds)
                    {
                        _context.CustomerAllergies.Add(new CustomerAllergy
                        {
                            CustomerId = customer.CustomerId,
                            ActiveIngredientId = allergyId
                        });
                    }
                }
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Your profile was updated successfully.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAccount()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Index", "Home");

            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null) return RedirectToAction("Login", "CustomerRegister");

            var user = _context.TblUsers.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(int UserId, string Password, string NewPassword, string ConfirmPassword)
        {
            var user = _context.TblUsers.FirstOrDefault(u => u.UserId == UserId);
            if (user == null) return RedirectToAction("Index", "Home");

            if (user.Password != Password)
            {
                ModelState.AddModelError("", "Current password is incorrect.");
                return View(user);
            }

            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError("", "New password and confirmation do not match.");
                return View(user);
            }

            user.Password = NewPassword;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Password updated successfully!";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult DownloadProfilePdf()
        {
            QuestPDF.Settings.License = LicenseType.Community; // Ensure Community license is set

            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null) return RedirectToAction("Login", "CustomerRegister");

            var user = _context.TblUsers.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerNavigation.UserId == user.UserId);
            var selectedAllergies = new List<string>();
            if (customer != null)
            {
                selectedAllergies = _context.CustomerAllergies
                    .Where(ca => ca.CustomerId == customer.CustomerId)
                    .Join(_context.ActiveIngredient,
                          ca => ca.ActiveIngredientId,
                          ai => ai.ActiveIngredientId,
                          (ca, ai) => ai.Ingredients)
                    .ToList();
            }

            // Path to logo image
            var logoPath = Path.Combine(_environment.WebRootPath, "images", "Logo_2-removebg-preview.png");

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // Header with logo and pharmacy name
                    page.Header()
                        .Column(headerCol =>
                        {
                            if (System.IO.File.Exists(logoPath))
                            {
                                headerCol.Item().Element(imgContainer =>
                                {
                                    imgContainer
                                        .AlignCenter()
                                        .MaxWidth(80)       // maximum width
                                        .MaxHeight(80)      // maximum height
                                        .Image(logoPath);
                                });


                            }

                            headerCol.Item().Text("IBHAYI PHARMACY")
                                .Bold()
                                .FontSize(20)
                                .FontColor(Colors.Blue.Darken1)
                                .AlignCenter();

                            headerCol.Item().Text("My Profile")
                                .SemiBold()
                                .FontSize(18)
                                .FontColor(Colors.Black)
                                .AlignCenter();

                            // Separator line
                            headerCol.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        });

                    // Profile details
                    page.Content()
                        .Column(col =>
                        {
                            void AddDetail(string label, string value)
                            {
                                col.Item().Row(row =>
                                {
                                    row.RelativeItem().Text(label).SemiBold();
                                    row.RelativeItem().Text(value);
                                });
                            }

                            AddDetail("First Name:", user.FirstName);
                            AddDetail("Last Name:", user.LastName);
                            AddDetail("ID Number:", user.Idnumber);
                            AddDetail("Phone Number:", user.PhoneNumber);
                            AddDetail("Email:", user.Email);

                            col.Item().PaddingTop(10).Text("Allergies:").SemiBold();
                            if (selectedAllergies.Any())
                            {
                                foreach (var allergy in selectedAllergies)
                                {
                                    col.Item().Text($"• {allergy}");
                                }
                            }
                            else
                            {
                                col.Item().Text("No allergies listed.");
                            }
                        });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", "ProfileDetails.pdf");
        }

    }

}


