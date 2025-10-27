using IBayiLibrary.Models.Domain;
using IBayiLibrary.Repository;
using Microsoft.AspNetCore.Mvc;
using ONT_PROJECT.Models;
using System.Data;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace ONT_PROJECT.Controllers
{
    public class OrderController : Controller
    {

        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        private readonly EmailService _emailService;

        public OrderController(IOrderRepository orderRepository, EmailService emailService, IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _emailService = emailService;
            _userRepository = userRepository;

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
                var date = order;
                date.DatePlaced = DateTime.Now;
                //var dateReceived = order;
                //dateReceived.DateReceived = null;
                var status = order;
                status.Status = "Walk-in";
                var vat = order;
                vat.VAT = 15;
                var pharmacist = order;
                var userId = HttpContext.Session.GetInt32("UserId");
                var user = await _userRepository.GetPharmacistByID(userId.Value);
                // if user found, build full nam                 
                pharmacist.PharmacistID = user.UserID;
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
            return RedirectToAction("GetOrdersMedication", "Order");
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
        [HttpPost]
        public async Task<IActionResult> Update(tblOrder order)
        {
            var date = order;
            date.DateRecieved = DateTime.Now;

            var person = await _orderRepository.GetOrdersByID(order.OrderID);
            var success = await _orderRepository.UpdateOrder(order.OrderID, order.Status, order.DateRecieved);

            if (!string.IsNullOrEmpty(person.Email))
            {
                string emailBody = $@"
                        <p>Hello {person.FirstName}<br>{person.LastName}</p>                       
                        <p>Your Order is ready for collection</p>
                        <p><strong>#Order ID:</strong> {person.OrderID}</p>                      
                        <p><strong>Medicine:</strong> {person.MedicineName}</p>
                        <p><strong>Date Placed:</strong> {person.DatePlaced}</p>  
                        <p><strong>Quantity:</strong> {person.Quantity}</p>             
                        <p><strong>Total:</strong> {person.LineTotal}</p>  
                        <p><strong>Dispensed On:</strong> {DateTime.Now:yyyy-MM-dd}</p>";


                _emailService.Send(person.Email, "Your order has been collected", emailBody);
            }
            return RedirectToAction("GetOrdersMedication", new { id = order.OrderID });
        }
        public async Task<IActionResult> Pack()
        {
            var success = await _orderRepository.PackOrder();
            return View(success);
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePackOrders(int[] medicineIds)
        {
            if (medicineIds != null && medicineIds.Any())
            {
                foreach (var id in medicineIds)
                {
                    await _orderRepository.UpdatePackOrder(id); // this now matches MedicineID
                }
            }

            return RedirectToAction("Pack");
        }


    }
}