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
        Task<bool> AddAsync(Prescriptions prescription);
        
        Task<IEnumerable<tblUser>> GetCustomerName();
        Task<IEnumerable<Doctor>> GetDoctorName();
        Task<IEnumerable<tblUser>> GetCutomerIDNo();
        Task<IEnumerable<PrescriptionModel>> GetUnproccessedPrescriptions();
        Task<tblUser> GetCustomerByIDs(int UserID);
        Task<PrescriptionModel> GetPrescriptionByID(int id);
        Task<bool> UpdatePrescription(Prescriptions prescriptions);
        Task<Prescriptions> FindPrescription(int PrescriptionID);
     
    }
}
