using IBayiLibrary.Repository;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace ONT_PROJECT.Controllers
{
    public class UnproccessedPrescriptionController : Controller
    {
        private readonly IUnproccessedPrescriptionRepository _unproccessedprescriptionRepository;
        
        public UnproccessedPrescriptionController(IUnproccessedPrescriptionRepository unproccessedprescriptionRepository)
        {
            _unproccessedprescriptionRepository = unproccessedprescriptionRepository;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> UnprocessedPrescription()
        {
            var fridge = await _unproccessedprescriptionRepository.GetUnproccessedPrescriptions();
            return View(fridge);
        }              
       


    }
}
