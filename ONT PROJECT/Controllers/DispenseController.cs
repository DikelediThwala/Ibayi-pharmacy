using IBayiLibrary.Models.Domain;
using IBayiLibrary.Repository;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
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
                .GroupBy(r => new { r.FirstName, r.Name, r.Date, r.Status, r.Repeats, r.RepeatsLeft,r.IDNumber })
                .Select(g => new PrescriptionModel
                {
                    FirstName = g.Key.FirstName,
                    Name = g.Key.Name,
                    Date = g.Key.Date,
                    Status = g.Key.Status,
                    Repeats = g.Key.Repeats,
                    RepeatsLeft = g.Key.RepeatsLeft,
                    IDNumber = g.Key.IDNumber,
                    // combine medications + quantities
                    MedicineName = string.Join(", ", g.Select(x => x.MedicineName + " (" + x.Quantity + ")")),
                    PrescriptionLineID = g.First().PrescriptionLineID // one ID for action
                })
                .ToList();

            return View(grouped);
        }

        [HttpPost]
        public async Task<IActionResult> Process(int id,tblUser user,PrescriptionModel prescription)
        {
            
            var prescriptionToUpdate = new PrescriptionModel
            {
                PrescriptionID = id
            };

            bool success = await _prescriptionRepository.UpdateDispnse(prescriptionToUpdate);

            if (success)
            {
                var resetLink = Url.Action("ResetPassword", "User", new { email = user.Email }, protocol: Request.Scheme);
                string emailBody = $@" <p>Hello {user.FirstName},</p>
            <p>Your Prescription has been dispensed:</p>
           <p><strong>{prescription.MedicineName}</strong></p>
           <p><strong>{prescription.Repeats}</strong></p>
           <p><strong>{prescription.RepeatsLeft}</strong></p>
           <p><strong>{prescription.Quantity}</strong></p>";
                _emailService.Send(user.Email, "Your Medication", emailBody);
                return Json(new { success = true, message = "Prescription dispensed." });
            }
                
            else
                return Json(new { success = false, message = "Failed to ." });


           
        }
    }
}
