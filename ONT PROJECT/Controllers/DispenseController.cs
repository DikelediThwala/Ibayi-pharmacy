using IBayiLibrary.Models.Domain;
using IBayiLibrary.Repository;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace ONT_PROJECT.Controllers
{
    public class DispenseController : Controller
    {
        private readonly IPrescriptionLineRepository _lineRepository;

        public DispenseController(IPrescriptionLineRepository lineRepository)
        {
            _lineRepository = lineRepository;
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
                .GroupBy(r => new { r.FirstName, r.Name, r.Date, r.Status, r.Repeats, r.RepeatsLeft })
                .Select(g => new PrescriptionModel
                {
                    FirstName = g.Key.FirstName,
                    Name = g.Key.Name,
                    Date = g.Key.Date,
                    Status = g.Key.Status,
                    Repeats = g.Key.Repeats,
                    RepeatsLeft = g.Key.RepeatsLeft,
                    // combine medications + quantities
                    MedicineName = string.Join(", ", g.Select(x => x.MedicineName + " (" + x.Quantity + ")")),
                    PrescriptionLineID = g.First().PrescriptionLineID // one ID for action
                })
                .ToList();

            return View(grouped);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
