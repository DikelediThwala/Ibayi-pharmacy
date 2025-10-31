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
        // POST: Update Profile - FIXED WITH EMAIL UPDATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(TblUser model, List<int> SelectedAllergyIds, string RemoveProfilePicture)
        {
            try
            {
                var email = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(email))
                    return RedirectToAction("Login", "CustomerRegister");

                // Get the existing user from database
                var user = _context.TblUsers
                    .Include(u => u.Customer)
                        .ThenInclude(c => c.CustomerAllergies)
                    .FirstOrDefault(u => u.Email == email);

                if (user == null)
                    return NotFound();

                // MANUAL VALIDATION LIKE REGISTRATION - Don't rely on ModelState.IsValid

                // Check required fields manually
                var validationErrors = new List<string>();

                if (string.IsNullOrWhiteSpace(model.FirstName))
                    validationErrors.Add("First name is required.");

                if (string.IsNullOrWhiteSpace(model.LastName))
                    validationErrors.Add("Last name is required.");

                if (string.IsNullOrWhiteSpace(model.Idnumber))
                    validationErrors.Add("ID number is required.");

                if (string.IsNullOrWhiteSpace(model.PhoneNumber))
                    validationErrors.Add("Phone number is required.");

                if (string.IsNullOrWhiteSpace(model.Email))
                    validationErrors.Add("Email is required.");

                // Check if email is being changed and if new email already exists (excluding current user)
                if (!string.IsNullOrWhiteSpace(model.Email) && model.Email.Trim() != user.Email)
                {
                    bool emailExists = _context.TblUsers.Any(u => u.Email.ToLower() == model.Email.Trim().ToLower() && u.UserId != user.UserId);
                    if (emailExists)
                    {
                        validationErrors.Add("This email is already registered by another user.");
                    }
                }

                // If there are validation errors, return to edit mode
                if (validationErrors.Any())
                {
                    TempData["ErrorMessage"] = string.Join(" ", validationErrors);
                    return RedirectToEditModeWithData(user, SelectedAllergyIds, model);
                }

                // UPDATE FIELDS - INCLUDING EMAIL
                user.FirstName = model.FirstName.Trim();
                user.LastName = model.LastName.Trim();
                user.Idnumber = model.Idnumber.Trim();
                user.PhoneNumber = model.PhoneNumber.Trim();
                user.Email = model.Email.Trim(); // ADD THIS LINE

                // Handle profile picture
                if (RemoveProfilePicture == "true")
                {
                    user.ProfilePicture = null;
                }
                else if (model.ProfileFile != null && model.ProfileFile.Length > 0)
                {
                    using var ms = new MemoryStream();
                    model.ProfileFile.CopyTo(ms);
                    user.ProfilePicture = ms.ToArray();
                }

                // Update allergies - Same pattern as registration
                if (user.Customer != null)
                {
                    // Remove existing allergies
                    _context.CustomerAllergies.RemoveRange(user.Customer.CustomerAllergies);

                    // Add new selected allergies
                    if (SelectedAllergyIds != null && SelectedAllergyIds.Any())
                    {
                        foreach (var allergyId in SelectedAllergyIds.Distinct())
                        {
                            user.Customer.CustomerAllergies.Add(new CustomerAllergy
                            {
                                CustomerId = user.Customer.CustomerId,
                                ActiveIngredientId = allergyId
                            });
                        }
                    }
                }

                // Save changes
                _context.SaveChanges();

                // Update session with new email if it was changed
                if (user.Email != email)
                {
                    HttpContext.Session.SetString("UserEmail", user.Email);
                }

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating your profile. Please try again.";
                return RedirectToAction("Index", new { edit = true });
            }
        }

        private IActionResult RedirectToEditModeWithData(TblUser user, List<int> selectedAllergyIds, TblUser model = null)
        {
            // Use the model data for form fields, but user data for other properties
            var viewModel = new TblUser
            {
                UserId = user.UserId,
                FirstName = model?.FirstName ?? user.FirstName,
                LastName = model?.LastName ?? user.LastName,
                Idnumber = model?.Idnumber ?? user.Idnumber,
                PhoneNumber = model?.PhoneNumber ?? user.PhoneNumber,
                Email = model?.Email ?? user.Email, // ADD THIS LINE
                ProfilePicture = user.ProfilePicture,
                Role = user.Role,
                Title = user.Title,
                Status = user.Status
            };

            // Repopulate view data for the edit view - Same as registration
            var allAllergies = _context.ActiveIngredient
                .Select(ai => new SelectListItem
                {
                    Value = ai.ActiveIngredientId.ToString(),
                    Text = ai.Ingredients
                })
                .OrderBy(a => a.Text)
                .ToList();

            ViewBag.SelectedAllergies = selectedAllergyIds ?? new List<int>();
            ViewBag.ActiveIngredients = allAllergies;
            ViewBag.EditMode = true;

            return View("Index", viewModel);
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
