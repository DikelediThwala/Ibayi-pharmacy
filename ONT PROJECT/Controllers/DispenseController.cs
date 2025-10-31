using IBayiLibrary.Models.Domain;
using IBayiLibrary.Repository;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using ONT_PROJECT.Models;
using System.Text;
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
                .GroupBy(r => new { r.FirstName, r.Name, r.Date, r.Status, r.Repeats, r.RepeatsLeft, r.Quantity, r.Instructions, r.IDNumber, r.PrescriptionID, r.Email, })
                .Select(g => new PrescriptionModel
                {
                    FirstName = g.Key.FirstName,
                    Name = g.Key.Name,
                    Date = g.Key.Date,
                    Status = g.Key.Status,
                    Repeats = g.Key.Repeats,
                    RepeatsLeft = g.Key.RepeatsLeft,
                    Quantity = g.Key.Quantity,
                    Instructions = g.Key.Instructions,
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
        public async Task<IActionResult> Process(int[] medicineIds, string searchTerm, PrescriptionViewModel prescriptionss)
        {
            if (medicineIds != null && medicineIds.Any())
            {
                var medicationRows = new StringBuilder();

                foreach (var id in medicineIds)
                {
                    // Update the prescription as dispensed
                    await _prescriptionRepository.UpdateDispnse(id);

                    // Retrieve details for each medicine
                    var medicine = await _prescriptionRepository.GetMedicineDetailsByIdAsync(id);

                    if (medicine != null)  // <-- use the medicine details, not prescriptionss
                    {
                        medicationRows.Append($@"
                        <tr>
                            <td style='border: 1px solid #ccc; padding: 8px;'>{medicine.MedicineName}</td>
                            <td style='border: 1px solid #ccc; padding: 8px;'>{medicine.Instructions}</td>
                            <td style='border: 1px solid #ccc; padding: 8px; text-align: center;'>{medicine.Repeats}</td>
                            <td style='border: 1px solid #ccc; padding: 8px; text-align: center;'>{medicine.RepeatsLeft}</td>
                        </tr>");
                    }
                }

                if (!string.IsNullOrEmpty(prescriptionss.Email))
                {
                    string emailBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; color: #333;'>
                        <p>Hello {prescriptionss.FirstName},</p>                  
                        <p>Your medication(s) have been successfully <strong>dispensed</strong>.</p>

                        <table style='border-collapse: collapse; width: 100%; max-width: 700px;'>
                            <thead>
                                <tr style='background-color: #f2f2f2;'>
                                    <th style='border: 1px solid #ccc; padding: 8px; text-align: left;'>Medication</th>
                                    <th style='border: 1px solid #ccc; padding: 8px; text-align: left;'>Instructions</th>
                                    <th style='border: 1px solid #ccc; padding: 8px; text-align: center;'>Repeats</th>
                                    <th style='border: 1px solid #ccc; padding: 8px; text-align: center;'>Repeats Left</th>
                                </tr>
                            </thead>
                            <tbody>
                                {medicationRows}
                            </tbody>
                        </table>

                        <p style='margin-top: 20px;'>
                            <strong>Dispensed On:</strong> {DateTime.Now:yyyy-MM-dd}
                        </p>

                        <p>Thank you for choosing <strong>IBAYI PHARMACY</strong>.</p>
                        <p>Stay healthy,<br><strong>GRP-04-04Pharmacy Team</strong></p>
                    </body>
                    </html>";
                    _emailService.Send(prescriptionss.Email, "GRP-04-04: Medication Dispensed", emailBody);
                }
            }
            return RedirectToAction("DispensePrescription", new { searchTerm });
        }

    }
}
