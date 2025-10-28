using IBayiLibrary.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Repository
{
    public interface IDoctorRepository
    {
        Task<bool> AddAsync(Doctor doctor);
        Task<bool> CheckEmailExistsAsync(string email);
        

    }
}
