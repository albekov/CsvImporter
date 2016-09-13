using System.Collections.Generic;
using System.Threading.Tasks;
using CsvImport.Model;

namespace CsvImport.Database
{
    public interface IDbManager
    {
        void Init();
        Task<int> AddRecords(ICollection<People> records);
    }
}