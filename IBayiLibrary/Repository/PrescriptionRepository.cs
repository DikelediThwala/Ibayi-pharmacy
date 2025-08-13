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
        public async Task<bool> AddAsync(Prescriptions prescription)
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
        //public async Task<PrescriptionModel> GetPrescriptionByID(int id)
        //{
        //    IEnumerable<Prescriptions> result = await _db.GetData<Prescriptions, dynamic>("spGetPrescriptionByID", new { PrescriptionID = id });
        //    return result.FirstOrDefault();


        public async Task<PrescriptionModel> GetPrescriptionByID(int id)
        {
            var result = await _db.GetData<PrescriptionModel, dynamic>(
                "spGetPrescriptionByID",
                new { PrescriptionID = id }
            );
            
            return result.FirstOrDefault();
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

    }
}
