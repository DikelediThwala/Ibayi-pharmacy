using Dapper;
using IBayiLibrary.DataAccess;
using IBayiLibrary.Models.Domain;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
        public async Task<IEnumerable<tblUser>> GetCustomerName()
        {
            string query = "spGetCustomerNames";
            return await _db.GetData<tblUser, dynamic>(query, new { });
        }
        public async Task<IEnumerable<Doctor>> GetDoctorName()
        {
            string query = "sp_GetDoctorNames";
            return await _db.GetData<Doctor, dynamic>(query, new { });
        }
        public async Task<bool> AddAsync(PrescriptionViewModel prescription)
        {
            try
            {
                await _db.SaveData("spInsertPrescription", new { prescription.Date, prescription.CustomerID, prescription.PharmacistID, prescription.PrescriptionPhoto, prescription.DoctorID });
                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AddPrescLineAsync(PrescriptionViewModel prescription)
        {
            try
            {
                await _db.SaveData("spInsertPrescriptionLine", new { prescription.MedicineID, prescription.PrescriptionID, prescription.Instructions, prescription.Quantity, prescription.Repeats, prescription.RepeatsLeft });
                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<IEnumerable<tblUser>> GetCutomerIDNo()
        {
            string query = "spGetCustomerIDNo";
            return await _db.GetData<tblUser, dynamic>(query, new { });
        }
       
        public async Task<tblUser> GetCustomerByIDs(int UserID)
        {
            IEnumerable<tblUser> result = await _db.GetData<tblUser, dynamic>("GetCustomerByID", new { ID = UserID });
            return result.FirstOrDefault();
                    
        }
        public async Task<Prescriptions> FindPrescription(int id)
        {
            IEnumerable<Prescriptions> result = await _db.GetData<Prescriptions, dynamic>("spDownloadPrescription", new { PrescriptionID = id });
            return result.FirstOrDefault();

        }
        public async Task<PrescriptionViewModel> GetPrescriptionByID(int id)
        {
            IEnumerable<PrescriptionViewModel> result = await _db.GetData<PrescriptionViewModel, dynamic>("spGetPrescriptionByID", new { PrescriptionID = id });
            return result.FirstOrDefault();
        }


        public async Task<bool> UpdateDispnse(PrescriptionModel prescriptions)
        {
            try
            {
                await _db.SaveData("spUpdateDispense",
                    new { PrescriptionID = prescriptions.PrescriptionID });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> UpdatePrescription(Prescriptions prescriptions)
        {
            try
            {

                await _db.SaveData("spUpdatePrescription", prescriptions);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<IEnumerable<PrescriptionLines>> GetLastPrescriptioRow()
        {
            string query = "spGetLastPrescriptioRow";
            return await _db.GetData<PrescriptionLines, dynamic>(query, new { });
        }
        public async Task<IEnumerable<PrescriptionModel>> GetLastPrescriptions()
        {
            string query = "spGetProccessedPrescrption";
            return await _db.GetData<PrescriptionModel, dynamic>(query, new { });
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
        public async Task<PrescriptionModel> GetDispenseById(int prescriptionId)
        {
            string query = "spDispenseByID";
            var result = await _db.GetData<PrescriptionModel, dynamic>(
                query,
                new { PrescriptionID = prescriptionId }
            );
            return result.FirstOrDefault();
        }
    }
}
