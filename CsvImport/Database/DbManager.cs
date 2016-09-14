using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CsvImport.Model;

namespace CsvImport.Database
{
    public class DbManager : IDbManager
    {
        public async Task Init()
        {
            await Task.Factory.StartNew(MigrationHelper.InitDatabase);
        }

        public async Task<int> InsertRecords(ICollection<People> records)
        {
            using (var context = new Context())
            {
                context.Peoples.AddRange(records);
                await context.SaveChangesAsync();
                return records.Count;
            }
        }

        public async Task<PagedResult<People>> Load(Query query)
        {
            query = query ?? new Query();

            try
            {
                using (var context = new Context())
                {
                    IQueryable<People> queryable = context.Peoples;

                    var result = await query.Apply(queryable);

                    return result;
                }
            }
            catch (DbException ex)
            {
                throw new ImporterDbException(ex.Message);
            }
            catch (DataException ex)
            {
                throw new ImporterDbException(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<People> Load(int id)
        {
            using (var context = new Context())
            {
                return await context.Peoples.FirstOrDefaultAsync(p => p.Id == id);
            }
        }

        public async Task Update(People people)
        {
            using (var context = new Context())
            {
                var record = await context.Peoples.FirstOrDefaultAsync(p => p.Id == people.Id);

                record.FullName = people.FullName;
                record.BirthDate = people.BirthDate;
                record.Email = people.Email;
                record.Phone = people.Phone;

                await context.SaveChangesAsync();
            }
        }

        public async Task Delete(int id)
        {
            using (var context = new Context())
            {
                var record = await context.Peoples.FirstOrDefaultAsync(p => p.Id == id);
                context.Peoples.Remove(record);

                await context.SaveChangesAsync();
            }
        }
    }
}