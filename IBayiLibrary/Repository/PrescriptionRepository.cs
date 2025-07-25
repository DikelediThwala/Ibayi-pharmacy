using IBayiLibrary.DataAccess;
using IBayiLibrary.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Repository
{
    public class PrescriptionRepository: IPrescriptionRepository
    {
        private readonly ISqlDataAccess _db;

        public PrescriptionRepository(ISqlDataAccess db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Customer>> GetCustomerName()
        {
            string query = "spGetCustomerNames";
            return await _db.GetData<Customer, dynamic>(query, new { });
        }
        public async Task<bool> AddAsync(Prescription prescription)
        {
            try
            {
                await _db.SaveData("spInsertPrescription", new {prescription.Date,prescription.CustomerID,prescription.PharmacistID, prescription.PrescriptionPhoto,prescription.DoctorID});
                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
