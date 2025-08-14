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
    public class UnproccessedPrescriptionRepository: IUnproccessedPrescriptionRepository
    {
        private readonly ISqlDataAccess _db;

        public UnproccessedPrescriptionRepository(ISqlDataAccess db)
        {
            _db = db;
        }
        public async Task<IEnumerable<UnproccessedPrescription>> GetUnproccessedPrescriptions()
        {
            string query = "spGetUnproccessedPrescription";
            return await _db.GetData<UnproccessedPrescription, dynamic>(query, new { });
        }
        public async Task<bool> GetPrescByIDPrescription(int unprocessedPrescriptionId)
        {
            try
            {
                await _db.SaveData(
                    "spProcessPrescription",
                    new { UnproccessedPrescriptionID = unprocessedPrescriptionId, Status = "Processed" }
                );

                return true; // If no exception, assume success
            }
            catch
            {
                return false; // Something went wrong
            }
        }
    }
}
