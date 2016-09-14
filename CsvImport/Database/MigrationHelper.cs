using System;
using System.Data;
using System.Data.Common;
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
            try
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
            catch (DbException ex)
            {
                throw new ImporterDbException(ex.Message);
            }
            catch (DataException ex)
            {
                throw new ImporterDbException(ex.InnerException?.Message ?? ex.Message);
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