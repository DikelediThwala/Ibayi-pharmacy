using IBayiLibrary.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ONT_PROJECT.Controllers
{
    public class PharmacistReportController : Controller
    {
        private readonly IPrescriptionLineRepository _prescriptionLineRepository;
        public PharmacistReportController(IPrescriptionLineRepository prescriptionLineRepository)
        {         
            _prescriptionLineRepository = prescriptionLineRepository;
        }

        public async Task<IActionResult> PharmacistGenerateReport(string groupBy = "FullName")
        {
            var data = await _prescriptionLineRepository.GenerateReport();
            ViewBag.GroupBy = groupBy;
            return View(data);
        }

    }
}
