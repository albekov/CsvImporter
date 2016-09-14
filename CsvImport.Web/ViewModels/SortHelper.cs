using System;
using System.Collections.Generic;
using System.Linq;
using CsvImport.Database;

namespace CsvImport.Web.ViewModels
{
    public static class SortHelper
    {
        public static SortingStatement[] ParseSortString(this string sortString)
        {
            var sortings = (sortString ?? string.Empty)
                .Split(new[] {'-'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim().Split(new[] {'_'}, StringSplitOptions.RemoveEmptyEntries))
                .Where(a => a.Length == 2)
                .Select(a => new SortingStatement(a[0], a[1].Equals("true", StringComparison.OrdinalIgnoreCase)))
                .ToArray();

            return sortings;
        }

        public static string ToSortString(this IEnumerable<SortingStatement> sortings)
        {
            var sortString = string.Join("-", sortings.Select(s => $"{s.PropertyName}_{s.Ascending.ToString().ToLower()}"));
            return sortString;
        }

        public static IEnumerable<SortingStatement> AppendSorting(this ICollection<SortingStatement> sortings, string propertyName)
        {
            var sorting = sortings
                .FirstOrDefault(s => s.PropertyName.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

            var newSortings = sortings
                .Where(s => !s.PropertyName.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var currentSortOrder = sorting?.Ascending;
            var newSortOrder = currentSortOrder == null
                ? true
                : currentSortOrder == true
                    ? false
                    : (bool?)null;

            if (newSortOrder.HasValue)
                newSortings.Add(new SortingStatement(propertyName, newSortOrder.Value));

            return newSortings;
        }
    }
}