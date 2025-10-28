using IBayiLibrary.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Repository
{
    public interface IUserRepository
    {
        Task<bool> AddAsync(tblUser user);
        Task<IEnumerable<PrescriptionViewModel>> GetCustomers();
        Task<int> NoOfCustomer();
        Task<tblUser> GetByIdAsync(int id);
        Task<tblUser> GetPharmacistByID(int id);
        Task<bool> CheckIDNumberExistsAsync(string idNumber);
        
        

    }
}
