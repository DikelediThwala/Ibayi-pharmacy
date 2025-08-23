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
        public async Task<IActionResult> CreatePrescriptions()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePrescriptions(PrescriptionViewModel prescription)
        {
            try
            {

                if (prescription.PescriptionFile != null && prescription.PescriptionFile.Length > 0)
                {
                    // Open stream and validate PDF header
                    var stream = prescription.PescriptionFile.OpenReadStream();
                    using var reader = new BinaryReader(stream);
                    var header = reader.ReadBytes(4);

                    // Reset stream position so we can read it again below
                    stream.Position = 0;

                    if (Encoding.ASCII.GetString(header) != "%PDF")
                    {
                        ModelState.AddModelError("PescriptionFile", "This is not a valid PDF file.");
                        return View(prescription);
                    }

                    // Copy full file to memory
                    using var ms = new MemoryStream();
                    await prescription.PescriptionFile.CopyToAsync(ms);
                    prescription.PrescriptionPhoto = ms.ToArray();
                }

                //var role = prescription;
                //role.PharmacistID = 1009;
                //var status = prescription;
                //status.Status = "Proccessed";
                //var repLeft = prescription;
                //repLeft.RepeatsLeft = repLeft.Repeats;
                bool addPerson = await _prescriptionRepository.AddAsync(prescription);
                if (addPerson)
                {
                    TempData["msg"] = "Sucessfully Added";
                }
                else
                {
                    TempData["msg"] = "Could not add";
                }

                //var result = await _prescriptionLineRepository.GetLastPrescriptioRow();
                //var lastRow = result.FirstOrDefault();

                //int prescriptionID = lastRow?.PrescriptionID ?? 0;
                //prescription.PrescriptionID = prescriptionID;
                //bool addPrescLine = await _prescriptionRepository.AddPrescLineAsync(prescription);

                //if (addPrescLine)
                //{
                //    TempData["msg"] = "Sucessfully Added";
                //}
                //else
                //{
                //    TempData["msg"] = "Could not add";
                //}             
            }
            catch (Exception ex)
            {
                TempData["msg"] = " Something went wrong!!!";
            }
            return RedirectToAction("CreateUser", "Pharmacist");
        }



    }
}
