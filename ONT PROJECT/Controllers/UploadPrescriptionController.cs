using IBayiLibrary.Models.Domain;
using IBayiLibrary.Repository;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
//using ONT_PROJECT.Models;
//using iText.Kernel.Pdf;
//using iText.Layout;
//using iTextSharp.text;
//using iTextSharp.text.pdf;
//using System.IO;
//using iText.Layout.Element;
//using iText.IO.Image;
using System.IO;
using System.Text;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace ONT_PROJECT.Controllers
{
    public class UploadPrescriptionController : Controller
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IPrescriptionLineRepository _prescriptionLineRepository;
        public UploadPrescriptionController(IPrescriptionRepository prescriptionRepository, IPrescriptionLineRepository prescriptionLineRepository)
        {
            _prescriptionRepository = prescriptionRepository;
            _prescriptionLineRepository = prescriptionLineRepository;
        }
        public async Task<IActionResult> CreatePrescForWalkins()
        {
            var customerRequests = await _prescriptionRepository.GetCustomerName();
            ViewBag.UserID = new SelectList(customerRequests.Select(c => new { c.UserID, FullName = c.FirstName + " " + c.LastName }), "UserID", "FullName");
            var doc = await _prescriptionRepository.GetDoctorName();
            ViewBag.DoctorID = new SelectList(doc.Select(c => new { c.DoctorID, FullName = c.Name + " " + c.Surname }), "DoctorID", "FullName");
            var prescLine = await _prescriptionLineRepository.GetMedicineName();
            ViewBag.MedicineID = new SelectList(prescLine.Select(prescLine => new { prescLine.MedicineID, prescLine.MedicineName }), "MedicineID", "MedicineName");


            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePrescForWalkins(PrescriptionViewModel prescription)
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

                var role = prescription;
                role.PharmacistID = 1009;
                var status = prescription;
                status.Status = "Proccessed";
                var repLeft = prescription;
                repLeft.RepeatsLeft = repLeft.Repeats;
                bool addPerson = await _prescriptionRepository.AddAsync(prescription);
                if (addPerson)
                {
                    TempData["msg"] = "Sucessfully Added";
                }
                else
                {
                    TempData["msg"] = "Could not add";
                }

                var result = await _prescriptionLineRepository.GetLastPrescriptioRow();
                var lastRow = result.FirstOrDefault();

                int prescriptionID = lastRow?.PrescriptionID ?? 0;
                prescription.PrescriptionID = prescriptionID;
                bool addPrescLine = await _prescriptionRepository.AddPrescLineAsync(prescription);

                if (addPrescLine)
                {
                    TempData["msg"] = "Sucessfully Added";
                }
                else
                {
                    TempData["msg"] = "Could not add";
                }

                var customerRequests = await _prescriptionRepository.GetCustomerName();
                ViewBag.UserID = new SelectList(customerRequests.Select(c => new { c.UserID, FullName = c.FirstName + " " + c.LastName }), "UserID", "FullName");
                var doc = await _prescriptionRepository.GetDoctorName();
                ViewBag.DoctorID = new SelectList(doc.Select(c => new { c.DoctorID, FullName = c.Name + " " + c.Surname }), "DoctorID", "FullName");
                var prescLine = await _prescriptionLineRepository.GetMedicineName();
                ViewBag.MedicineID = new SelectList(prescLine.Select(prescLine => new { prescLine.MedicineID, prescLine.MedicineName }), "MedicineID", "MedicineName");
            }
            catch (Exception ex)
            {
                TempData["msg"] = " Something went wrong!!!";
            }
            return RedirectToAction("CreateUser", "Pharmacist");
        }

        public async Task<IActionResult> CreatePrescription()
        {         
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePrescription(PrescriptionViewModel prescription)
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

                var role = prescription;
                role.PharmacistID = 1009;
                var status = prescription;
                status.Status = "Proccessed";
                var repLeft = prescription;
                repLeft.RepeatsLeft = repLeft.Repeats;
                bool addPerson = await _prescriptionRepository.AddAsync(prescription);
                if (addPerson)
                {
                    TempData["msg"] = "Sucessfully Added";
                }
                else
                {
                    TempData["msg"] = "Could not add";
                }

                var result = await _prescriptionLineRepository.GetLastPrescriptioRow();
                var lastRow = result.FirstOrDefault();

                int prescriptionID = lastRow?.PrescriptionID ?? 0;
                prescription.PrescriptionID = prescriptionID;
                bool addPrescLine = await _prescriptionRepository.AddPrescLineAsync(prescription);

                if (addPrescLine)
                {
                    TempData["msg"] = "Sucessfully Added";
                }
                else
                {
                    TempData["msg"] = "Could not add";
                }             
            }
            catch (Exception ex)
            {
                TempData["msg"] = " Something went wrong!!!";
            }
            return RedirectToAction("CreateUser", "Pharmacist");
        }
        public async Task<IActionResult> DownloadPrescription(int id)
        {
            //var prescription = await _prescriptionRepository.FindPrescription(id);
            //if (prescription == null || prescription.PrescriptionPhoto == null)
            //{
            //    return NotFound();
            //}

            //using var memoryStream = new MemoryStream();
            //var document = new Document();
            //var writer = PdfWriter.GetInstance(document, memoryStream);

            //document.Open();

            //using var imageStream = new MemoryStream(prescription.PrescriptionPhoto);
            //var image = iTextSharp.text.Image.GetInstance(imageStream);
            //image.ScaleToFit(500f, 700f);
            //image.Alignment = Element.ALIGN_CENTER;
            //document.Add(image);
            //document.Close();

            //var fileName = $"Prescription_{id}.pdf";
            //return File(memoryStream.ToArray(), "application/pdf", fileName);
            var prescription = await _prescriptionRepository.FindPrescription(id);
            if (prescription == null || prescription.PrescriptionPhoto == null)
            {
                return NotFound();
            }

            // No need to recreate a PDF, just return the stored one
            var fileName = $"Prescription_{id}.pdf";
            return File(prescription.PrescriptionPhoto, "application/pdf", fileName);

        }
        public async Task<IActionResult> GetPrescriptions()
        {

            var p = await _prescriptionRepository.GetLastPrescriptions();
            return View(p);
        }   
        [HttpGet]
        public async Task<IActionResult> GetPrescByID(int id)
        {
            var prescription = await _prescriptionRepository.GetPrescriptionByID(id);
            if (prescription == null)
            {
                return NotFound();
            }

            // Return the form view
            return View("CreatePrescription", prescription);
        }

    }
}
