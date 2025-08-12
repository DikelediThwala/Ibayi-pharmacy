using IBayiLibrary.Models.Domain;
using IBayiLibrary.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ONT_PROJECT.Models;

namespace ONT_PROJECT.Controllers
{
    public class PrescriptionLineController : Controller
    {
        private readonly IPrescriptionLineRepository _prescriptionLineRepository;
        public PrescriptionLineController(IPrescriptionLineRepository prescriptionLineRepository)
        {            
            _prescriptionLineRepository = prescriptionLineRepository;
        }


        public async Task<IActionResult> CreatePrescriptionLine()
        {           
            var prescLine = await _prescriptionLineRepository.GetMedicineName();
            ViewBag.MedicineID = new SelectList(prescLine.Select(f=> new { f.MedicineID, f.MedicineName }), "MedicineID", "MedicineName");
            return View();


        }      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePrescriptionLine(PrescriptionLines prescriptionLi)
        {
            var result = await _prescriptionLineRepository.GetLastPrescriptioRow();
            var lastRow = result.FirstOrDefault();

            int prescriptionID = lastRow?.PrescriptionID ?? 0; 

            bool addPerson = await _prescriptionLineRepository.AddAsync(prescriptionLi);
            if (addPerson)
            {
                TempData["msg"] = "Sucessfully Added";
            }
            else
            {
                TempData["msg"] = "Could not add";
            }
            var prescLine = await _prescriptionLineRepository.GetMedicineName();
            ViewBag.MedicineID = new SelectList(prescLine.Select(prescLine => new { prescLine.MedicineID, prescLine.MedicineName }), "MedicineID", "MedicineName");
            return RedirectToAction("CreateDoctor", "Doctors");
        }
    }
}
