using IBayiLibrary.DataAccess;
using IBayiLibrary.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Repository
{
    public class OrderRepository : IOrderRepository
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
                await _db.SaveData("spOrderMedication", new
                {
                    tblOrder.CustomerID,
                    tblOrder.PharmacistID,
                    tblOrder.Status,
                    tblOrder.TotalDue,
                    tblOrder.VAT,
                    tblOrder.DatePlaced,
                    tblOrder.DateRecieved
                });
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
            return await _db.GetData<tblOrder, dynamic>(query, new { });
        }       
        public async Task<int> TotalNumberOfOrders()
        {
            string spName = "spTotalNumberOfOrders";
            return await _db.GetSingleValue<int, dynamic>(spName, new { });
        }
       
        public async Task<IEnumerable<tblOrder>> GetAllOrders()
        {
            string query = "spGetOrders";
            var orders = await _db.GetData<tblOrder, dynamic>(query, new { });
            return orders ?? new List<tblOrder>(); // fallback to empty list
        }

        public async Task<bool> UpdateOrder(int id, string status, DateTime? dateRecieved)
        {
            await _db.SaveData("spUpdateOrderStatus", new { OrderID = id, Status = status, DateRecieved = dateRecieved });
            return true;
        }

        public async Task<tblOrder> GetOrdersByID(int id)
        {
            IEnumerable<tblOrder> result = await _db.GetData<tblOrder, dynamic>("GetOrderByID", new { OrderID = id });
            return result.FirstOrDefault();
        }

    }
}
