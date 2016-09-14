using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CsvImport.Model;

namespace CsvImport.Database
{
    public class Query
    {
        public Query(int page, int? pageSize, params SortingStatement[] sortings)
        {
            Page = page;
            PageSize = pageSize;
            Sortings = sortings ?? new SortingStatement[0];
        }

        public Query()
            : this(1, null)
        {
        }

        public int? PageSize { get; }
        public int Page { get; }
        public ICollection<SortingStatement> Sortings { get; private set; }

        public async Task<PagedResult<People>> Apply(IQueryable<People> queryable)
        {
            var count = queryable.Count();

            Sortings = FixSortings<People>(Sortings);
            queryable = ApplySortings(queryable, Sortings);

            var result = new PagedResult<People>
            {
                Page = 1,
                PageSize = null,
                Pages = 1,
                Count = count
            };

            if (PageSize.HasValue)
            {
                var pageSize = Math.Max(10, Math.Min(PageSize.Value, 10000));
                var pages = (count - 1)/pageSize + 1;
                var page = Math.Max(1, Math.Min(Page, pages));
                var skip = (page - 1)*pageSize;

                queryable = queryable
                    .Skip(skip)
                    .Take(pageSize);

                result.Page = page;
                result.PageSize = pageSize;
                result.Pages = pages;
            }

            result.Sortings = Sortings;
            result.Records = await queryable.ToListAsync();
            return result;
        }

        private static IQueryable<People> ApplySortings(IQueryable<People> queryable, ICollection<SortingStatement> sortings)
        {
            if ((sortings != null) && sortings.Any())
                for (var i = 0; i < sortings.Count; i++)
                {
                    var sorting = sortings.ElementAt(i);
                    queryable = AddSorting(queryable, sorting.PropertyName, sorting.Ascending, i == 0);
                }
            else
                queryable = queryable.OrderBy(r => r.Id);

            return queryable;
        }

        private static IOrderedQueryable<T> AddSorting<T>(IQueryable<T> source, string propertyName, bool ascending, bool first = true)
        {
            var param = Expression.Parameter(typeof(T), string.Empty);
            var property = Expression.PropertyOrField(param, propertyName);
            var sort = Expression.Lambda(property, param);
            var call = Expression.Call(
                typeof(Queryable),
                (first ? "OrderBy" : "ThenBy") + (ascending ? string.Empty : "Descending"),
                new[] {typeof(T), property.Type},
                source.Expression,
                Expression.Quote(sort));
            return (IOrderedQueryable<T>) source.Provider.CreateQuery<T>(call);
        }

        private static ICollection<SortingStatement> FixSortings<T>(ICollection<SortingStatement> sortings)
        {
            var properties = typeof(T).GetProperties().Select(p => p.Name).ToList();
            sortings = sortings.Where(s => properties.Contains(s.PropertyName)).ToList();
            return sortings;
        }
    }

    public class SortingStatement
    {
        public SortingStatement(string propertyName, bool ascending = true)
        {
            PropertyName = propertyName;
            Ascending = ascending;
        }

        public string PropertyName { get; set; }
        public bool Ascending { get; set; }
    }
}