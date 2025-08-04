using IBayiLibrary.DataAccess;
using IBayiLibrary.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Repository
{
    public class DoctorRepository:IDoctorRepository
    {
        private readonly ISqlDataAccess _db;

        public DoctorRepository(ISqlDataAccess db)
        {
            _db = db;
        }       
        public async Task<bool> AddAsync(Doctor doctor)
        {
            try
            {
                await _db.SaveData("spInsertDoctor", new { doctor.Name, doctor.Surname, doctor.PracticeNo, doctor.ContactNo, doctor.Email});
                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
