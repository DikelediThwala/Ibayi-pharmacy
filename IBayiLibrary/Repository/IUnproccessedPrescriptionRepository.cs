using IBayiLibrary.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Repository
{
    public interface IUnproccessedPrescriptionRepository
    {
        Task<IEnumerable<UnproccessedPrescription>> GetUnproccessedPrescriptions();
        public Task<bool> GetPrescByIDPrescription(int unprocessedPrescriptionId);
        public Task<PrescriptionViewModel> GetPrescriptionByID(int id);
        Task<int> NumberOfUnprocessedPresc();
        public Task<bool> UpdateUnprocessedPrescription(int id);
    }
}
