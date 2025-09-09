using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Controllers;
using ONT_PROJECT.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ONT_PROJECT.Controllers
{
    public class B_OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public B_OrderController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            SeedMedicines();  
        }

        public IActionResult Index(string tab)  
        {
            var vm = new NewOrderViewModel
            {
                Medicines = _context.Medicines.ToList(),
                BOrders = _context.BOrders
                     .Include(o => o.BOrderLines)
                     .ThenInclude(ol => ol.Medicine)
                     .ToList(),
                NewOrder = new BOrder()
            };

            ViewData["ActiveTab"] = tab ?? "stock"; 
            return View(vm);

        }



        [HttpPost]
        public async Task<IActionResult> MarkAsReceived(int id)
        {
            var order = await _context.BOrders.FindAsync(id);
            if (order == null)
                return NotFound();

            order.DateRecieved = DateOnly.FromDateTime(DateTime.Now);
            order.Status = "Received";

            await _context.SaveChangesAsync();

            var viewModel = new NewOrderViewModel
            {
                Medicines = _context.Medicines.ToList(),
                BOrders = _context.BOrders
                  .Include(o => o.BOrderLines)
                  .ThenInclude(ol => ol.Medicine)
                  .ToList(),
                NewOrder = new BOrder()
            };

            ViewData["ActiveTab"] = "orders";

            return View("Index", viewModel);
        }


        public IActionResult Create()
        {
            var medications = _context.Medicines
                          .Where(m => m.Status == "Active")
                          .Select(m => new SelectListItem
                          {
                              Value = m.MedicineId.ToString(),
                              Text = m.MedicineName
                          })
                          .ToList();

            ViewBag.Medications = medications;

            ViewBag.MedicationDetails = _context.Medicines
                                                .Where(m => m.Status == "Active")
                                                .ToList();
            ViewBag.MedicationPrices = _context.Medicines
                                               .Where(m => m.Status == "Active")
                                               .ToDictionary(m => m.MedicineId, m => m.SalesPrice);

            return View(new BOrder());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BOrder order)
        {
            ModelState.Remove("Status");

            if (!ModelState.IsValid)
            {
                var allErrors = ModelState
                    .SelectMany(kvp => kvp.Value.Errors.Select(e => new { Field = kvp.Key, Error = e.ErrorMessage }))
                    .ToList();

                return BadRequest(new { success = false, message = "Validation errors occurred.", errors = allErrors });
            }

            if (order == null || order.BOrderLines == null || !order.BOrderLines.Any())
            {
                return Json(new { success = false, message = "Please add at least one medication." });
            }

            // Filter only selected medications before saving
            order.BOrderLines = order.BOrderLines
                .Where(ol => ol.IsSelected && ol.Quantity > 0)
                .ToList();

            if (!order.BOrderLines.Any())
                return BadRequest(new { success = false, message = "Please select at least one medication." });

            order.DatePlaced = DateOnly.FromDateTime(DateTime.Now);
            order.Status = "Pending";

            _context.BOrders.Add(order);
            await _context.SaveChangesAsync();

            // Load saved order with related Medicines
            var savedOrder = await _context.BOrders
                .Include(o => o.BOrderLines!)
                    .ThenInclude(ol => ol.Medicine!)
                .FirstOrDefaultAsync(o => o.BOrderId == order.BOrderId);

            if (savedOrder != null)
            {
                // Group by supplier for email notifications
                var selectedOrderLines = savedOrder.BOrderLines;
                var groupsBySupplier = selectedOrderLines.GroupBy(ol => ol.Medicine!.SupplierId);

                var supplierIds = selectedOrderLines
                    .Select(ol => ol.Medicine!.SupplierId)
                    .Distinct()
                    .ToList();

                var suppliers = await _context.Suppliers
                    .Where(s => supplierIds.Contains(s.SupplierId))
                    .ToListAsync();

                // SMTP settings
                var smtpHost = _configuration["SmtpSettings:Host"];
                var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
                var smtpUser = _configuration["SmtpSettings:User"];
                var smtpPass = _configuration["SmtpSettings:Password"];
                var enableSsl = bool.Parse(_configuration["SmtpSettings:EnableSsl"]);

                using var smtpClient = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = enableSsl,
                };

                foreach (var group in groupsBySupplier)
                {
                    var supplierId = group.Key;
                    var supplier = suppliers.FirstOrDefault(s => s.SupplierId == supplierId);
                    if (supplier == null || string.IsNullOrEmpty(supplier.Email))
                        continue;

                    // Build email body
                    var emailBody = $@"
                <div style='font-family: Arial, sans-serif; color: #333;'>
                    <div style='text-align: center; margin-bottom: 20px;'>
                        <img src='https://i.imgur.com/fohBBIa.png' alt='Pharmacy Logo' style='max-width: 150px;' />
                        <h2 style='margin: 10px 0;'>Ibayi Pharmacy</h2>
                    </div>
                    <p>Dear {supplier.Name},</p>
                    <p>You have a new medication order:</p>
                    <p><strong>Order Number: {savedOrder.BOrderId}</strong>.</p>
                    <p>Medications ordered:</p>
                    <table style='width: 100%; border-collapse: collapse;'>
                        <thead>
                            <tr style='background-color: #f2f2f2;'>
                                <th style='border: 1px solid #ddd; padding: 8px;'>Medication</th>
                                <th style='border: 1px solid #ddd; padding: 8px;'>Quantity</th>
                            </tr>
                        </thead>
                        <tbody>";

                    foreach (var ol in group)
                    {
                        emailBody += $@"
                    <tr>
                        <td style='border: 1px solid #ddd; padding: 8px;'>{ol.Medicine!.MedicineName}</td>
                        <td style='border: 1px solid #ddd; padding: 8px;'>{ol.Quantity}</td>
                    </tr>";
                    }

                    emailBody += @"
                        </tbody>
                    </table>
                    <p>Please process this order as soon as possible.</p>
                    <p>Regards,<br />Ibayi Pharmacy</p>
                    <hr style='margin-top: 40px;' />
                    <small style='color: #888;'>400 Jam St.,Gqeberha, Summerstrand,SA | 083 754 5485 | Ibayipharmacy24@gmail.com</small>
                </div>";

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(smtpUser),
                        Subject = $"Medication Order #{savedOrder.BOrderId}",
                        Body = emailBody,
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(supplier.Email);

                    try
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                    }
                    catch
                    {
                        // Optional: log the exception
                    }
                }
            }
            return Json(new { success = true, message = "Order placed successfully!" });

        }



        private void SeedMedicines()
        {
            if (!_context.Medicines.Any())
            {
                _context.Medicines.AddRange(new List<Medicine>
                {
                    new Medicine { MedicineName = "Paracetamol" },
                    new Medicine { MedicineName = "Ibuprofen" }
                });
                _context.SaveChanges();
            }
        }

        public IActionResult TestMedicines()
        {
            int count = _context.Medicines.Count();
            return Content($"Medicines count: {count}");
        }

        public IActionResult AdjustStock(int id)
        {
            var medicine = _context.Medicines.FirstOrDefault(m => m.MedicineId == id);
            if (medicine == null)
                return NotFound();

            return PartialView("_AdjustStock", medicine);
        }
        [HttpPost]
        public IActionResult AdjustStock(int medicineId, int addQuantity = 0, int removeQuantity = 0, int resetQuantity = 0, int reorderLevel = 0)
        {
            var medicine = _context.Medicines.FirstOrDefault(m => m.MedicineId == medicineId);
            if (medicine == null)
                return NotFound();

            if (resetQuantity > 0)
            {
                medicine.Quantity = resetQuantity;
            }
            else
            {
                medicine.Quantity += addQuantity;
                medicine.Quantity -= removeQuantity;
                if (medicine.Quantity < 0) medicine.Quantity = 0;
            }

            medicine.ReorderLevel = reorderLevel;

            _context.SaveChanges();

            return Json(new { success = true, newQuantity = medicine.Quantity, newReorderLevel = medicine.ReorderLevel });
        }

    }
}
