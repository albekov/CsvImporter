using System.Data.Entity;
using CsvImport.Model;

namespace CsvImport.Database
{
    public class Context : DbContext
    {
        public Context() : base("DefaultConnection")
        {
        }

        public DbSet<People> Peoples { get; set; }
    }
}