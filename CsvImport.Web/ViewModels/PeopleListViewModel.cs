using System;
using System.Collections.Generic;
using System.Linq;
using CsvImport.Database;
using CsvImport.Model;

namespace CsvImport.Web.ViewModels
{
    public class PeopleListViewModel
    {
        public PeopleListViewModel()
        {
        }

        public PeopleListViewModel(PagedResult<People> result)
        {
            Result = result;
            Sortings = InitSortings(result.Sortings);
            SortString = result.Sortings.ToSortString();
        }

        public PagedResult<People> Result { get; set; }
        public string SortString { get; set; }
        public Dictionary<string, FieldSortViewModel> Sortings { get; set; }

        private Dictionary<string, FieldSortViewModel> InitSortings(ICollection<SortingStatement> sortings)
        {
            var result = new Dictionary<string, FieldSortViewModel>();

            CreatePropertySorting(result, sortings, nameof(People.FullName));
            CreatePropertySorting(result, sortings, nameof(People.BirthDate));
            CreatePropertySorting(result, sortings, nameof(People.Email));
            CreatePropertySorting(result, sortings, nameof(People.Phone));

            return result;
        }

        private void CreatePropertySorting(
            Dictionary<string, FieldSortViewModel> result,
            ICollection<SortingStatement> sortings,
            string propertyName)
        {
            var sorting = sortings.FirstOrDefault(s => s.PropertyName.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

            result[propertyName] = new FieldSortViewModel
            {
                Order = sorting?.Ascending,
                SortString = sortings.AppendSorting(propertyName).ToSortString()
            };
        }

        public class FieldSortViewModel
        {
            public bool? Order { get; set; }
            public string SortString { get; set; }
        }
    }
}