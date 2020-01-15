using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SchoolManagement.Infrastructure
{
    public interface ITableExtension
    {
        string SortName { get; set; }
        TableExtension.SortOrder SortOrder { get; set; }
    }

    public static class TableExtension
    {
        public const string SeparatorField = "--";

        public static IQueryable<T> SafeWhere<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> func,
            object obj)
        {
            if (obj != null) return queryable.Where(func);
            return queryable;
        }

        public static IQueryable<T> SortQuery<T>(this IQueryable<T> queryable, ITableExtension sort)
        {
            return SortQuery(queryable, sort.SortName, sort.SortOrder);
        }

        public static IQueryable<T> SortQuery<T>(this IQueryable<T> queryable, string key, SortOrder dir)
        {
            if (string.IsNullOrEmpty(key))
            {
                return queryable;
            }

            var property = GetProperty<T>(key);


            if (dir == SortOrder.Ascending)
            {
                queryable = queryable.OrderBy(x => property.GetValue(x, null)).AsQueryable();
            }
            else
            {
                queryable = queryable.OrderByDescending(x => property.GetValue(x, null)).AsQueryable();
            }


            return queryable;
        }

        private static PropertyInfo GetProperty<T>(string key)
        {
            string fieldName = key;
            if (key.Contains(SeparatorField))
            {
                fieldName = key.Split(SeparatorField)[0];
            }

            var property = typeof(T).GetProperty(fieldName);
            return property;
        }

        public static SortOrder Toggle(this SortOrder sortOrder)
        {
            return sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
        }

        public static T WithTableParameters<T>(this T current, ITableExtension other)
            where T : ITableExtension
        {
            current.SortName = other.SortName;
            current.SortOrder = other.SortOrder;
            return current;
        }

        public enum SortOrder
        {
            Ascending,
            Descending
        }
    }
}
