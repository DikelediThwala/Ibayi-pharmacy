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
        Task<bool> AddAsync(Prescription prescription);
        
        Task<IEnumerable<Customer>> GetCustomerName();

    }
}
