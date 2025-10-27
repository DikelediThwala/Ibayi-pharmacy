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

        // GET: Profile Settings Overview
        [HttpGet]
        public IActionResult Settings()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login", "CustomerRegister");

            var user = _context.TblUsers.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();

            return View(user);
        }

        // GET: Edit Profile
        [HttpGet]
        public IActionResult Index(bool? edit)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login", "CustomerRegister");

            var user = _context.TblUsers
                .Include(u => u.Customer)
                    .ThenInclude(c => c.CustomerAllergies)
                .FirstOrDefault(u => u.Email == email);

            if (user == null) return NotFound();

            // Selected allergies
            var selectedAllergies = user.Customer?.CustomerAllergies
                .Select(ca => ca.ActiveIngredientId)
                .ToList() ?? new List<int>();

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

        // POST: Update Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(TblUser model, List<int> SelectedAllergyIds, string RemoveProfilePicture)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login", "CustomerRegister");

            var user = _context.TblUsers
                .Include(u => u.Customer)
                    .ThenInclude(c => c.CustomerAllergies)
                .FirstOrDefault(u => u.Email == email);

            if (user == null) return NotFound();

            // Update required fields safely
            user.FirstName = model.FirstName ?? user.FirstName;
            user.LastName = model.LastName ?? user.LastName;
            user.Idnumber = model.Idnumber ?? user.Idnumber;
            user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;

            // Handle profile picture
            if (RemoveProfilePicture == "true")
                user.ProfilePicture = null;
            else if (model.ProfileFile != null && model.ProfileFile.Length > 0)
            {
                using var ms = new MemoryStream();
                model.ProfileFile.CopyTo(ms);
                user.ProfilePicture = ms.ToArray();
            }

            // Update allergies
            if (user.Customer != null)
            {
                _context.CustomerAllergies.RemoveRange(user.Customer.CustomerAllergies);

                if (SelectedAllergyIds != null)
                {
                    foreach (var id in SelectedAllergyIds)
                    {
                        user.Customer.CustomerAllergies.Add(new CustomerAllergy
                        {
                            CustomerId = user.Customer.CustomerId,
                            ActiveIngredientId = id
                        });
                    }
                }
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Index");
        }

        // POST: Delete Account
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAccount()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login", "CustomerRegister");

            var user = _context.TblUsers
                .Include(u => u.Customer)
                    .ThenInclude(c => c.CustomerAllergies)
                .FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                if (user.Customer != null)
                {
                    _context.CustomerAllergies.RemoveRange(user.Customer.CustomerAllergies);
                    _context.Customers.Remove(user.Customer);
                }

                _context.TblUsers.Remove(user);
                _context.SaveChanges();
            }

            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // GET: Change Password
        [HttpGet]
        public IActionResult ChangePassword()
        {
            ViewData["ActiveTab"] = "password";
            return View(new ChangePasswordViewModel());
        }

        // POST: Change Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login", "CustomerRegister");

            var user = _context.TblUsers.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();

            if (user.Password != model.CurrentPassword)
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                return View(model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "New password and confirmation do not match.");
                return View(model);
            }

            user.Password = model.NewPassword;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Password updated successfully!";
            return RedirectToAction("ChangePassword");
        }

        // GET: Download Profile as PDF
        [HttpGet]
        public IActionResult DownloadProfilePdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login", "CustomerRegister");

            var user = _context.TblUsers
                .Include(u => u.Customer)
                    .ThenInclude(c => c.CustomerAllergies)
                        .ThenInclude(ca => ca.ActiveIngredient)
                .FirstOrDefault(u => u.Email == email);

            if (user == null) return NotFound();

            var allergies = user.Customer?.CustomerAllergies
                .Select(ca => ca.ActiveIngredient.Ingredients)
                .ToList() ?? new List<string>();

            var logoPath = Path.Combine(_environment.WebRootPath, "images", "Logo_2-removebg-preview.png");

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // Header
                    page.Header().Column(headerCol =>
                    {
                        if (System.IO.File.Exists(logoPath))
                        {
                            headerCol.Item().Element(imgContainer =>
                            {
                                imgContainer.AlignCenter()
                                            .MaxWidth(80)
                                            .MaxHeight(80)
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

                        headerCol.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    // Content
                    page.Content().Column(col =>
                    {
                        if (user.ProfilePicture != null && user.ProfilePicture.Length > 0)
                        {
                            col.Item().Element(pic =>
                            {
                                pic.AlignCenter()
                                   .MaxWidth(150)
                                   .MaxHeight(150)
                                   .Image(user.ProfilePicture);
                            });
                        }

                        void AddDetail(string label, string value)
                        {
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text(label).SemiBold();
                                row.RelativeItem().Text(value ?? "N/A");
                            });
                        }

                        AddDetail("First Name:", user.FirstName);
                        AddDetail("Last Name:", user.LastName);
                        AddDetail("ID Number:", user.Idnumber);
                        AddDetail("Phone Number:", user.PhoneNumber);
                        AddDetail("Email:", user.Email);

                        col.Item().PaddingTop(10).Text("Allergies:").SemiBold();
                        if (allergies.Any())
                        {
                            foreach (var allergy in allergies)
                                col.Item().Text($"• {allergy}");
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
