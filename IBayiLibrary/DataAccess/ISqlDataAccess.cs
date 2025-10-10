using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.DataAccess
{
    public interface ISqlDataAccess
    {
        Task<IEnumerable<T>> GetData<T, P>(string spName, P parameters, string connectionId = "DefaultConnection");
        Task SaveData<T>(string spName, T parameters, string connectionId = "DefaultConnection");
        Task<int> SaveDataWithReturn<T>(string spName, T parameters, string connectionId = "DefaultConnection");
        Task<T> GetSingleValue<T, P>(string spName, P parameters, string connectionId = "DefaultConnection");
    }
}
