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
        public async Task<PrescriptionViewModel> GetPrescriptionByID(int id)
        {
            IEnumerable<PrescriptionViewModel> result = await _db.GetData<PrescriptionViewModel, dynamic>("spGetUnprocessedPresciptionByID", new { UnprocessedPrescriptionID = id });
            return result.FirstOrDefault();
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
        public async Task<bool> UpdateUnprocessedPrescription(UnproccessedPrescription unproccessedPrescription)
        {
            try
            {
                await _db.SaveData("spUpdateUnprocessedPrescription",
                    new { UnprocessedPrescriptionID = unproccessedPrescription.UnprocessedPrescriptionID });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    }
}
