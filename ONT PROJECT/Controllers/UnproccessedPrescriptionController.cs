using IBayiLibrary.Models.Domain;
using IBayiLibrary.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Core.Types;
using System.Text;

namespace ONT_PROJECT.Controllers
{
    public class UnproccessedPrescriptionController : Controller
    {
        private readonly IUnproccessedPrescriptionRepository _unproccessedprescriptionRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IPrescriptionLineRepository _prescriptionLineRepository;
        public UnproccessedPrescriptionController(IUnproccessedPrescriptionRepository unproccessedprescriptionRepository,IPrescriptionRepository prescriptionRepository, IPrescriptionLineRepository prescriptionLineRepository)
        {
            _unproccessedprescriptionRepository = unproccessedprescriptionRepository;
            _prescriptionRepository = prescriptionRepository;
            _prescriptionLineRepository = prescriptionLineRepository;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetUnprocessedPrescription()
        {
            var fridge = await _unproccessedprescriptionRepository.GetUnproccessedPrescriptions();
            return View(fridge);
        }
        [HttpGet]
        public async Task<IActionResult> GetPrescByID(int id)
        {
            var prescLine = await _prescriptionLineRepository.GetMedicineName();
            ViewBag.MedicineID = new SelectList(prescLine.Select(prescLine => new { prescLine.MedicineID, prescLine.MedicineName }), "MedicineID", "MedicineName");
            var doc = await _prescriptionRepository.GetDoctorName();
            ViewBag.DoctorID = new SelectList(doc.Select(c => new { c.DoctorID, FullName = c.Name + " " + c.Surname }), "DoctorID", "FullName");

            var prescription = await _unproccessedprescriptionRepository.GetPrescriptionByID(id);
            if (prescription == null)
            {
                return NotFound();
            }           
            return View("~/Views/UploadPrescription/CreatePrescriptions.cshtml", prescription);
        }
    }
}
