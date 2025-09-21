using IBayiLibrary.Models.Domain;
using IBayiLibrary.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ONT_PROJECT.Controllers
{
    public class OrderController : Controller
    {
        
        private readonly IOrderRepository _orderRepository;
        private readonly EmailService _emailService;

        public OrderController(IOrderRepository orderRepository,EmailService emailService)
        {
            _orderRepository = orderRepository;
            _emailService = emailService;
        }

        public async Task<IActionResult> OrderMedication()
        {
            var results = await _orderRepository.MedicationOrder();
            return View(results);
        }
    }
}
