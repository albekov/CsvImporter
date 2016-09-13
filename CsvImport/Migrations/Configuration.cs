using System.Data.Entity.Migrations;
using CsvImport.Database;

namespace CsvImport.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Context context)
        {
        }
    }
}