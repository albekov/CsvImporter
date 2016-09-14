using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using CsvImport.Model;

namespace CsvImport.Database
{
    public class Context : DbContext
    {
        public Context() : base("DefaultConnection")
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 30;
        }

        public DbSet<People> Peoples { get; set; }
    }
}