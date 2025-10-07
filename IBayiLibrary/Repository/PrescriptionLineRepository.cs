using IBayiLibrary.DataAccess;
using IBayiLibrary.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Repository
{
    public class PrescriptionLineRepository:IPrescriptionLineRepository
    {
        private readonly ISqlDataAccess _db;

        public PrescriptionLineRepository(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<bool> AddAsync(PrescriptionLines prescriptionLine)
        {
            try
            {
                await _db.SaveData("spInsertPrescriptionLine", new { prescriptionLine.PrescriptionID, prescriptionLine.MedicineID, prescriptionLine.Quantity, prescriptionLine.Instructions, prescriptionLine.Repeats,prescriptionLine.RepeatsLeft });
                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<IEnumerable<Medicine>> GetMedicineName()
        {
            string query = "spGetMedicineName";
            return await _db.GetData<Medicine, dynamic>(query, new { });
        }
        public async Task<IEnumerable<PrescriptionViewModel>> GenerateReport()
        {
            string query = "spGenerateReports";
            return await _db.GetData<PrescriptionViewModel, dynamic>(query, new { });
        }
        public async Task<IEnumerable<PrescriptionLines>> GetLastPrescriptioRow()
        {
            string query = "spGetLastPrescriptioRow";
            return await _db.GetData<PrescriptionLines, dynamic>(query, new { });
        }
        public async Task<IEnumerable<PrescriptionModel>> SearchPrescriptions(string searchTerm)
        {
            string query = "spDispensePrescription";
            return await _db.GetData<PrescriptionModel, dynamic>(query, new { SearchTerm = searchTerm });
        }

    }
}
