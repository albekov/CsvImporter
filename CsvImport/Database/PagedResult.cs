using System.Collections.Generic;

namespace CsvImport.Database
{
    public class PagedResult<T>
    {
        public int Count { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Pages { get; set; }
        public ICollection<T> Records { get; set; }
    }
}