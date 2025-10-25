using IBayiLibrary.DataAccess;
using IBayiLibrary.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Repository
{
    public class UserRepository:IUserRepository
    {
        private readonly ISqlDataAccess _db;

        public UserRepository(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<bool> AddAsync(tblUser user)
        {
            try
            {
                await _db.SaveData("spInsertUser", new {user.FirstName,user.LastName,user.Email,user.Password,user.IDNumber,user.Role,user.PhoneNumber,user.Title });
                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<IEnumerable<PrescriptionViewModel>> GetCustomers()
        {
            string query = "spGetCustomers";
            return await _db.GetData<PrescriptionViewModel, dynamic>(query, new { });
        }
        public async Task<int> NoOfCustomer()
        {
            string spName = "spReadyOrder";
            return await _db.GetSingleValue<int, dynamic>(spName, new { });
        }
        public async Task<tblUser> GetByIdAsync(int id)
        {
            IEnumerable<tblUser> result = await _db.GetData<tblUser, dynamic>("spgetUserByID", new { UserID = id });
            return result.FirstOrDefault();
        }
        public async Task<tblUser> GetPharmacistByID(int id)
        {
            IEnumerable<tblUser> result = await _db.GetData<tblUser, dynamic>("spGetPharmacistID", new { UserID = id });
            return result.FirstOrDefault();
        }

    }
}
