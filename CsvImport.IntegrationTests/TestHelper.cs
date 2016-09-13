using System;
using System.Collections.Generic;
using System.IO;
using CsvImport.Model;

namespace CsvImport.IntegrationTests
{
    public class TestHelper : IDisposable
    {
        private readonly List<string> _files = new List<string>();

        private readonly Random _random = new Random();

        public void Dispose()
        {
            foreach (var fileName in _files)
                File.Delete(fileName);
        }

        public string CreateFile(int rows)
        {
            var fileName = $"temp_{_random.Next(10000)}.csv";
            _files.Add(fileName);

            var lines = new List<string>();

            for (int i = 0; i < rows; i++)
                lines.Add(PeopleToString(new People
                {
                    FullName = $"user {i}",
                    BirthDate = new DateTime(1980,1,1).AddDays(i).ToShortDateString(),
                    Email = $"user{i}@mail.ru",
                    Phone = $"+7 495 {i:0000000}"
                }));

            File.WriteAllLines(fileName, lines);

            return fileName;
        }

        private static string PeopleToString(People people)
        {
            return $"{people.FullName}\t{people.BirthDate}\t{people.Email}\t{people.Phone}";
        }
    }
}