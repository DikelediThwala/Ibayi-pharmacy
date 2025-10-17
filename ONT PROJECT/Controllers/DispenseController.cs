using IBayiLibrary.Models.Domain;
using IBayiLibrary.Repository;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using ONT_PROJECT.Models;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace ONT_PROJECT.Controllers
{
    public class DispenseController : Controller
    {
        private readonly IPrescriptionLineRepository _lineRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly EmailService _emailService;

        public DispenseController(IPrescriptionLineRepository lineRepository, IPrescriptionRepository prescriptionRepository, EmailService emailService)
        {
            _lineRepository = lineRepository;
            _prescriptionRepository = prescriptionRepository;
            _emailService = emailService;
        }

        public async Task<IActionResult> DispensePrescription(string searchTerm)
        {
            ViewBag.SearchTerm = searchTerm;

            if (string.IsNullOrEmpty(searchTerm))
            {
                return View(new List<PrescriptionModel>());
            }

            var results = await _lineRepository.SearchPrescriptions(searchTerm);

            // Group by Customer + Doctor + Date
            var grouped = results
                .GroupBy(r => new { r.FirstName, r.Name, r.Date, r.Status, r.Repeats, r.RepeatsLeft,r.IDNumber,r.PrescriptionID,r.Email })
                .Select(g => new PrescriptionModel
                {
                    FirstName = g.Key.FirstName,
                    Name = g.Key.Name,
                    Date = g.Key.Date,
                    Status = g.Key.Status,
                    Repeats = g.Key.Repeats,
                    RepeatsLeft = g.Key.RepeatsLeft,
                    IDNumber = g.Key.IDNumber,
                    PrescriptionID = g.Key.PrescriptionID,
                    Email = g.Key.Email,
                    // combine medications + quantities
                    MedicineName = string.Join(", ", g.Select(x => x.MedicineName + " (" + x.Quantity + ")")),
                    PrescriptionLineID = g.First().PrescriptionLineID // one ID for action
                })
                .ToList();

            return View(grouped);
        }

        [HttpPost]
        public async Task<IActionResult> Process(int id, PrescriptionViewModel prescription)
        {
            var prescriptionss = await _prescriptionRepository.GetDispenseById(id);

            var prescriptionToUpdate = new PrescriptionModel
            {
                PrescriptionID = id
            };
            bool success = await _prescriptionRepository.UpdateDispnse(prescriptionToUpdate);
            if (success)
            {
                var allergicIngredients = await _prescriptionRepository.GetAllergicIngredients(prescriptionss.CustomerID);

                if (allergicIngredients.Any())
                {
                    // You can map IDs to names for clarity if needed
                    TempData["AllergyAlert"] = "Warning: Patient is allergic to the following ingredients: "
                                               + string.Join(", ", allergicIngredients);

                }
                if (!string.IsNullOrEmpty(prescriptionss.Email))
                {
                    string emailBody = $@"
                        <p>Hello {prescriptionss.FirstName},</p>
                        <p>Your prescription has been dispensed successfully.</p>
                        <p><strong>Medication(s):</strong> {prescriptionss.MedicineName}</p>
                        <p><strong>Repeats:</strong> {prescriptionss.Repeats}</p>
                        <p><strong>Repeats Left:</strong> {prescriptionss.RepeatsLeft}</p>
                        <p><strong>Quantity:</strong> {prescriptionss.Quantity}</p>                       
                        <p><strong>Dispensed On:</strong> {DateTime.Now:yyyy-MM-dd}</p>";


                    _emailService.Send(prescriptionss.Email, "Your Medication Has Been Dispensed", emailBody);
                   
                }
                return Json(new { success = true, message = "Prescription dispensed." });
            }
                                                         
            else
                return Json(new { success = false, message = "Failed to ." });         
        }
    }
}
