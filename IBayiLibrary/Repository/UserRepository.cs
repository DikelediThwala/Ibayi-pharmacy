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
       
    }
}
