//using IBayiLibrary.Models.Domain;
//using IBayiLibrary.Repository;
//using Microsoft.AspNetCore.Mvc;
//using ONT_PROJECT.Models;
//using static ONT_PROJECT.Controllers.PharmacistController;

//namespace ONT_PROJECT.Controllers
//{
//    public class UploadPrescriptionController : Controller
//    {
//        private readonly IPrescriptionRepository _prescriptionRepository;

//        public UploadPrescriptionController(IPrescriptionRepository prescriptionRepository)
//        {
//            _prescriptionRepository = prescriptionRepository;

//        }
//        public IActionResult LoadPresciption()
//        {
//            return View();
//        }
//        [HttpPost]
//        public async Task< IActionResult> LoadPresciption(Prescription prescription)
//        {
//            try
//            {               
//                bool addPerson = await _prescriptionRepository.AddAsync(prescription);
//                if (addPerson)
//                {
//                    TempData["msg"] = "Sucessfully Added";
//                }
//                else
//                {
//                    TempData["msg"] = "Could not add";
//                }
//            }
//            catch (Exception ex)
//            {
//                TempData["msg"] = " Something went wrong!!!";
//            }
//            return RedirectToAction("Index");
//        }
//    }
//}
