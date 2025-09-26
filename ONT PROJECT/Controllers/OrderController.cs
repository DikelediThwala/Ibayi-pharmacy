using IBayiLibrary.Models.Domain;
using IBayiLibrary.Repository;
using Microsoft.AspNetCore.Mvc;
using ONT_PROJECT.Models;

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

        public IActionResult CreateOrder()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder(tblOrder order)
        {
            try
            {
                var date= order;
                date.DatePlaced = DateTime.Now;
                //var dateReceived = order;
                //dateReceived.DateReceived = null;
                var status = order;
                status.Status = "Placed";
                var vat = order;
                vat.VAT = 15;
                var pharmacist = order;
                pharmacist.PharmacistID = 1009;
                bool addPerson = await _orderRepository.AddOrder(order);
                if (addPerson)
                {
                    TempData["msg"] = "Sucessfully Added";
                }
                else
                {
                    TempData["msg"] = "Could not add";
                }

                //OrderLine
                bool ad = await _orderRepository.AddOrderLine(order);
                if (ad)
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
            return RedirectToAction("LoadPrescription", "Pharmacist");
        }
        public async Task<IActionResult> OrderMedication()
        {
            var results = await _orderRepository.MedicationOrder();
            return View(results);
        }
        public async Task<IActionResult> GetOrdersMedication()
        {
            var results = await _orderRepository.GetAllOrders();
            return View(results);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var person = await _orderRepository.GetOrdersByID(id);
            return View(person);
        }

    }
}
