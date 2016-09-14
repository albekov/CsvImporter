using System.Linq;

namespace CsvImport.Model
{
    public static class CsvHelper
    {
        public static People FromCsv(this string line)
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

        public static string ToCsv(this People record)
        {
            if (record == null)
                return string.Empty;

            return $"{record.FullName}\t{record.BirthDate}\t{record.Email}\t{record.Phone}";
        }
    }
}