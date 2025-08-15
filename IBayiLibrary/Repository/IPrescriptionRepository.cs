using IBayiLibrary.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Repository
{
    public interface IPrescriptionRepository
    {
        Task<bool> AddAsync(PrescriptionViewModel prescription);
        Task<bool>AddPrescLineAsync(PrescriptionViewModel prescription);
        //public Task<bool> AddPrescriptionAsync(Prescriptions prescription);
        Task<IEnumerable<tblUser>> GetCustomerName();
        Task<IEnumerable<Doctor>> GetDoctorName();
        Task<IEnumerable<tblUser>> GetCutomerIDNo();
        Task<IEnumerable<PrescriptionLines>> GetLastPrescriptioRow();
        Task<IEnumerable<PrescriptionModel>> GetLastPrescriptions();
        Task<tblUser> GetCustomerByIDs(int UserID);
        Task<PrescriptionModel> GetPrescriptionByID(int id);
        Task<bool> UpdatePrescription(Prescriptions prescriptions);
        Task<Prescriptions> FindPrescription(int PrescriptionID);
        


    }
}
