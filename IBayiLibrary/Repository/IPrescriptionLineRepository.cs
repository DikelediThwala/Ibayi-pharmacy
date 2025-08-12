using IBayiLibrary.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Repository
{
    public interface IPrescriptionLineRepository
    {
       Task<bool> AddAsync(PrescriptionLines prescriptionLine);

        Task<IEnumerable<Medicine>>GetMedicineName();
        Task<IEnumerable<PrescriptionLines>> GetLastPrescriptioRow();

    }
}
