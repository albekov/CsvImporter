using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Linq;
using CsvImport.Migrations;
using NLog;

namespace CsvImport.Database
{
    public class MigrationHelper : MigrationsLogger
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static void InitDatabase()
        {
            var migrator = new DbMigrator(new Configuration());
            var decorator = new MigratorLoggingDecorator(migrator, new MigrationHelper());
            var pendingMigrations = migrator.GetPendingMigrations().ToList();

            if (pendingMigrations.Any())
            {
                Log.Info($"Found {pendingMigrations.Count} migrations.");
                decorator.Update();
            }
        }

        public override void Info(string message)
        {
            Log.Info(message);
        }

        public override void Warning(string message)
        {
            Log.Warn(message);
        }

        public override void Verbose(string message)
        {
            Log.Trace(message);
        }
    }
}