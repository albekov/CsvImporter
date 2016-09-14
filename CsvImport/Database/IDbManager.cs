using System.Collections.Generic;
using System.Threading.Tasks;
using CsvImport.Model;

namespace CsvImport.Database
{
    public interface IDbManager
    {
        /// <summary>
        /// Create and/or migrate database.
        /// </summary>
        /// <returns></returns>
        Task Init();

        /// <summary>
        /// Insert records into database.
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        Task<int> InsertRecords(ICollection<People> records);

        /// <summary>
        /// Load records with paging support.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PagedResult<People>> Load(int page, int pageSize);

        /// <summary>
        /// Load record by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<People> Load(int id);

        /// <summary>
        /// Update record in database.
        /// </summary>
        /// <param name="people"></param>
        /// <returns></returns>
        Task Update(People people);
    }
}