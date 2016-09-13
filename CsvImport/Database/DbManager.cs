using System.Collections.Generic;
using System.Threading.Tasks;
using CsvImport.Model;

namespace CsvImport.Database
{
    public class DbManager : IDbManager
    {
        public void Init()
        {
            MigrationHelper.InitDatabase();
        }

        public async Task<int> AddRecords(ICollection<People> records)
        {
            using (var context = new Context())
            {
                context.Peoples.AddRange(records);
                await context.SaveChangesAsync();
                return records.Count;
            }
        }
    }
}