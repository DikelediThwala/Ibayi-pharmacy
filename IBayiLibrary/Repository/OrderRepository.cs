using IBayiLibrary.DataAccess;
using IBayiLibrary.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Repository
{
    public class OrderRepository:IOrderRepository
    {
        private readonly ISqlDataAccess _db;

        public OrderRepository(ISqlDataAccess db)
        {
            _db = db;
        }
        public async Task<bool> AddOrder(tblOrder tblOrder)
        {
            try
            {
                await _db.SaveData("spOrderMedication", new {tblOrder.CustomerID,tblOrder.PharmacistID,tblOrder.Status,tblOrder.TotalDue,tblOrder.VAT,              
                    tblOrder.DatePlaced,tblOrder.DateReceived});
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> AddOrderLine(tblOrder tblOrder)
        {
            try
            {
                await _db.SaveData("spInsertOrderLine", new
                {
                    tblOrder.OrderID,
                    tblOrder.MedicineID,
                    tblOrder.Quantity,                  
                    tblOrder.Price,
                    tblOrder.LineTotal,                   
                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<IEnumerable<tblOrder>> MedicationOrder()
        {
            string query = "spPrepareOrder";
            return await _db.GetData<tblOrder, dynamic > (query, new { });
        }
        public async Task<IEnumerable<tblOrder>> GetAllOrders()
        {
            string query = "spGetOrders";
            return await _db.GetData<tblOrder, dynamic>(query, new { });
        }
        public async Task<bool> UpdateOrder(tblOrder order)
        {
            try
            {

                await _db.SaveData("spUpdateOrderStatus", order);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<tblOrder> GetOrdersByID(int id)
        {
            IEnumerable<tblOrder> result = await _db.GetData<tblOrder, dynamic>("GetOrderByID", new { OrderID = id });
            return result.FirstOrDefault();
        }
    }
}
