using IBayiLibrary.Models.Domain;
using IBayiLibrary.Repository;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Text;

namespace ONT_PROJECT.Controllers
{
    public class UnproccessedPrescriptionController : Controller
    {
        private readonly IUnproccessedPrescriptionRepository _unproccessedprescriptionRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;

        public UnproccessedPrescriptionController(IUnproccessedPrescriptionRepository unproccessedprescriptionRepository,IPrescriptionRepository prescriptionRepository)
        {
            _unproccessedprescriptionRepository = unproccessedprescriptionRepository;
            _prescriptionRepository = prescriptionRepository;
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
            var prescription = await _unproccessedprescriptionRepository.GetPrescriptionByID(id);
            if (prescription == null)
            {
                return NotFound();
            }           
            return View("~/Views/UploadPrescription/CreatePrescriptions.cshtml", prescription);
        }

        [HttpPost]
        public async Task<IActionResult> Process(int id)
        {
            var prescriptionToUpdate = new UnproccessedPrescription
            {
                UnprocessedPrescriptionID = id
            };

            bool success = await _unproccessedprescriptionRepository.UpdateUnprocessedPrescription(prescriptionToUpdate);

            if (success)
                return Json(new { success = true, message = "Prescription processed." });
            else
                return Json(new { success = false, message = "Failed to process." });
        }
       


       


    }
}
