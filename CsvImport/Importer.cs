using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvImport.Database;
using CsvImport.Model;
using NLog;

namespace CsvImport
{
    public class Importer
    {
        private readonly IDbManager _dbManager;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public int BatchSize { get; set; } = 1000;

        public Importer(IDbManager dbManager)
        {
            _dbManager = dbManager;
        }

        /// <summary>
        /// Prepare and migrate database.
        /// </summary>
        /// <returns></returns>
        public async Task Init()
        {
            await _dbManager.Init();
        }

        /// <summary>
        /// Load records from CSV file into database.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<int> ImportAsync(string fileName)
        {
            var records = new List<People>(BatchSize);
            var insertedCount = 0;
            using (var streamReader = new StreamReader(fileName, GetEncoding(fileName)))
            {
                while (true)
                {
                    var line = await streamReader.ReadLineAsync();
                    if (line == null)
                        break;

                    var record = ParseLine(line);
                    if (record != null)
                        records.Add(record);

                    if (records.Count >= BatchSize)
                    {
                        insertedCount += await _dbManager.InsertRecords(records);
                        records = new List<People>(BatchSize);
                    }
                }
            }

            insertedCount += await _dbManager.InsertRecords(records);

            return insertedCount;
        }

        private static People ParseLine(string line)
        {
            var array = line.Split('\t').Select(s => s.Trim()).ToList();
            if (!array.Any())
                return null;

            var people = new People();
            people.FullName = array[0];
            if (array.Count > 1)
                people.BirthDate = array[1];
            if (array.Count > 2)
                people.Email = array[2];
            if (array.Count > 3)
                people.Phone = array[3];

            return people;
        }


        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// http://stackoverflow.com/questions/3825390/effective-way-to-find-any-files-encoding 
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        private static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.GetEncoding(1251);
        }
    }
}