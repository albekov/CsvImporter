using System.Collections.Generic;
using System.Threading.Tasks;
using CsvImport.Database;
using CsvImport.Model;
using Moq;
using Xunit;

namespace CsvImport.IntegrationTests
{
    public class ImporterTests
    {
        [Fact]
        public void BatchInsertTest()
        {
            using (var helper = new TestHelper())
            {
                var dbManager = new Mock<IDbManager>();
                var result = InitFakeDbManager(dbManager);

                var importer = new Importer(dbManager.Object);
                importer.BatchSize = 100;

                importer.ImportAsync(helper.CreateFile(999)).Wait();

                Assert.Equal(10, result.Calls);
                Assert.Equal(999, result.Records);
            }
        }

        [Fact]
        public void InsertRecordsTest()
        {
            using (var helper = new TestHelper())
            {
                var dbManager = new Mock<IDbManager>();
                var result = InitFakeDbManager(dbManager);

                var importer = new Importer(dbManager.Object);

                Assert.Equal(0, result.Calls);
                Assert.Equal(0, result.Records);

                importer.ImportAsync(helper.CreateFile(100)).Wait();

                Assert.Equal(1, result.Calls);
                Assert.Equal(100, result.Records);

                importer.ImportAsync(helper.CreateFile(200)).Wait();

                Assert.Equal(2, result.Calls);
                Assert.Equal(300, result.Records);

                importer.ImportAsync(helper.CreateFile(300)).Wait();

                Assert.Equal(3, result.Calls);
                Assert.Equal(600, result.Records);
            }
        }

        private static DbStatistics InitFakeDbManager(Mock<IDbManager> dbManager)
        {
            var result = new DbStatistics();
            dbManager
                .Setup(o => o.InsertRecords(It.IsAny<ICollection<People>>()))
                .Callback<ICollection<People>>(r =>
                {
                    result.Calls++;
                    result.Records += r.Count;
                })
                .Returns<ICollection<People>>(r => Task.FromResult(r.Count));
            return result;
        }

        private class DbStatistics
        {
            public int Calls;
            public int Records;
        }
    }
}