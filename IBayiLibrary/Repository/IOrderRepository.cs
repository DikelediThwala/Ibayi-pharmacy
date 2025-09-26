using IBayiLibrary.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Repository
{
    public interface IOrderRepository
    {
        Task<bool> AddOrder(tblOrder tblOrder);
        Task<bool> AddOrderLine(tblOrder tblOrder);
        Task<IEnumerable<tblOrder>> MedicationOrder();
        Task<IEnumerable<tblOrder>> GetAllOrders();
        Task<bool> UpdateOrder(tblOrder order);
        Task<tblOrder> GetOrdersByID(int id);

    }
}
