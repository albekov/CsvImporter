using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvImport.Database;
using static System.Console;

namespace CsvImport.Console
{
    internal class Program
    {
        private static readonly Regex HelpArgument = new Regex(@"[\-/][hH?]");

        private static void Main(string[] args)
        {
            var files = ParseArguments(args);
            if (!files.Any())
            {
                PrintHelp();
                return;
            }

            WriteLine($"Importing {files.Count} files...");

            var importer = new Importer(new DbManager());
            importer.Init();

            var tasks = files
                .Select(file => ImportFile(importer, file))
                .ToArray();

            var results = Task.WhenAll(tasks).Result
                .Where(result => result > 0)
                .ToList();

            var recordsCount = results.Sum();

            WriteLine($"Successfully imported {recordsCount} records from {results.Count} of {files.Count} files");
        }

        private static async Task<int> ImportFile(Importer importer, string file)
        {
            try
            {
                WriteLine($"Import: {file}...");
                return await importer.ImportAsync(file);
            }
            catch (Exception ex)
            {
                WriteLine($"Failed to import {file}: {ex.Message}");
                return 0;
            }
        }

        private static List<string> ParseArguments(string[] args)
        {
            return args.Where(arg => !HelpArgument.IsMatch(arg)).ToList();
        }

        private static void PrintHelp()
        {
            WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} file1 file2 ...");
            WriteLine("\t-h, -? - Show this help");
            WriteLine();
        }
    }
}