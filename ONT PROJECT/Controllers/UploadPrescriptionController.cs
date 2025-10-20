using IBayiLibrary.Models.Domain;
using IBayiLibrary.Repository;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ONT_PROJECT.Models;
using System.IO;
using System.Text;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace ONT_PROJECT.Controllers
{
    public class UploadPrescriptionController : Controller
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IPrescriptionLineRepository _prescriptionLineRepository;
        private readonly IUnproccessedPrescriptionRepository _unproccessedprescriptionRepository;
        private readonly EmailService _emailService;
        public UploadPrescriptionController(IPrescriptionRepository prescriptionRepository, IPrescriptionLineRepository prescriptionLineRepository, IUnproccessedPrescriptionRepository unproccessedprescriptionRepository, EmailService emailService)
        {
            _prescriptionRepository = prescriptionRepository;
            _prescriptionLineRepository = prescriptionLineRepository;
            _unproccessedprescriptionRepository = unproccessedprescriptionRepository;
            _emailService = emailService;
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
        public async Task<IActionResult> CreatePrescriptions()
        {
           
            return View();
        }
        public async Task<IActionResult> CreatePrescForImmediateDispense()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePrescForImmediateDispense(PrescriptionViewModel prescription, int id)
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
                    // After adding prescription line
                    if (addPrescLine)
                    {
                        TempData["msg"] = "Successfully Added";

                        // Check for patient allergies
                        var allergicIngredients = await _prescriptionRepository.GetAllergicIngredients(prescription.CustomerID);

                        if (allergicIngredients.Any())
                        {
                            // You can map IDs to names for clarity if needed
                            TempData["AllergyAlert"] = "Warning: Patient is allergic to the following ingredients: "
                                                       + string.Join(", ", allergicIngredients);
                        }
                    }
                    else
                    {
                        TempData["msg"] = "Could not add prescription line";
                    }

                }
                else
                {
                    TempData["msg"] = "Could not add";
                }

            var prescId = prescription;
            prescId.PrescriptionID = prescId.PrescriptionID;
            var custId = prescription;
            custId.CustomerID = prescId.CustomerID;


            var prescriptionss = await _prescriptionRepository.GetDispenseById(repLeft.PrescriptionID);

            var prescriptionToUpdate = new PrescriptionModel
            {
                PrescriptionID = prescId.PrescriptionID
            };
            bool success = await _prescriptionRepository.UpdateDispnse(prescriptionToUpdate);
            if (success)
            {
                var allergicIngredients = await _prescriptionRepository.GetAllergicIngredients(prescId.CustomerID);

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
                return RedirectToAction("GetUnprocessedPrescription", "UnproccessedPrescription");
            }
            else
                return Json(new { success = false, message = "Failed to ." });


            
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
                status.Status = "Walk'ins";
                var repLeft = prescription;
                repLeft.RepeatsLeft = repLeft.Repeats;
                bool addPerson = await _prescriptionRepository.AddAsync(prescription);
                if (addPerson)
                {
                    TempData["msg"] = "Sucessfully Added";
                    var allergicIngredients = await _prescriptionRepository.GetAllergicIngredients(prescription.CustomerID);

                    if (allergicIngredients.Any())
                    {
                        // You can map IDs to names for clarity if needed
                        TempData["AllergyAlert"] = "Warning: Patient is allergic to the following ingredients: "
                                                   + string.Join(", ", allergicIngredients);
                    }
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
            return RedirectToAction("DispensePrescription", "Dispense");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePrescriptions(PrescriptionViewModel prescription,int id)
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
                    // After adding prescription line
                    if (addPrescLine)
                    {
                        TempData["msg"] = "Successfully Added";

                        // Check for patient allergies
                        var allergicIngredients = await _prescriptionRepository.GetAllergicIngredients(prescription.CustomerID);

                        if (allergicIngredients.Any())
                        {
                            // You can map IDs to names for clarity if needed
                            TempData["AllergyAlert"] = "Warning: Patient is allergic to the following ingredients: "
                                                       + string.Join(", ", allergicIngredients);
                        }
                    }
                    else
                    {
                        TempData["msg"] = "Could not add prescription line";
                    }

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
            return RedirectToAction("OrderMedication", "Order");
        }


           public async Task<IActionResult> DownloadPrescription(int id)
           {
                var prescription = await _prescriptionRepository.FindPrescription(id);
                if (prescription == null || prescription.PrescriptionPhoto == null)
                    return NotFound();

                var fileName = $"Prescription_{id}.pdf";

                // return raw PDF bytes
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
            return View("CreatePrescriptions","UploadPrescription");
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return Json(new { success = false, message = "No search term provided." });
            }

            var results = await _prescriptionRepository.SearchCustomer(searchTerm);

            var customer = results
                .GroupBy(r => new { r.FirstName, r.IDNumber })
                .Select(g => new PrescriptionViewModel
                {
                    FirstName = g.Key.FirstName,
                    IDNumber = g.Key.IDNumber
                })
                .FirstOrDefault();

            if (customer == null)
                return Json(new { success = false, message = "Customer not found." });

            return Json(new { success = true, data = customer });
        }
        [HttpGet]
        public async Task<IActionResult> GetCustomerDetails(int id)
        {
            var customer = await _prescriptionRepository.SelectCustomerName(id);
            if (customer == null)
            {
                return Json(new { success = false });
            }
            return Json(new
            {
                success = true,
                data = new { firstName = customer.FirstName,idNumber = customer.IDNumber }
            });
        }

        [HttpGet]
        public async Task<IActionResult> CreatePrescriptions(int id)
        {
            // Get the prescription (with the photo bytes)
            var prescription = await _prescriptionRepository.FindPrescription(id);

            if (prescription == null)
            {
                return NotFound();
            }

            // Pass it to your view model
            var viewModel = new PrescriptionViewModel
            {
                CustomerID = prescription.CustomerID,               
                Date = prescription.Date,
                PrescriptionPhoto = prescription.PrescriptionPhoto
            };

            return View(viewModel);
        }

    }
}
