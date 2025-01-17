using BookCatalog.Server.Domain.Attributes;
using System.Reflection;

namespace BookCatalog.Shared.Models
{
    /// <summary>
    /// Parses sort order passed by user into sort orders that can be used by OrderQueryBuilder
    /// </summary>
    public class SortMapper
    {
        public static List<AttributeSortOrder> MapSort<T>(IEnumerable<ClientSortDto> clientSort)
        {
            var props = typeof(T)
                .GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(SortableAttribute)));

            return MapSortToProps(clientSort, props);
        }

        public static List<AttributeSortOrder> MapSort<T1, T2>(IEnumerable<ClientSortDto> clientSort)
        {
            var props1 = typeof(T1)
                .GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(SortableAttribute)));

            var props2 = typeof(T2)
                .GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(SortableAttribute)));

            var props = props1.Concat(props2);

            return MapSortToProps(clientSort, props);
        }



        private static List<AttributeSortOrder> MapSortToProps(IEnumerable<ClientSortDto> clientSort, IEnumerable<PropertyInfo> props)
        {
            var result = new List<AttributeSortOrder>();

            if (clientSort is null || !clientSort.Any())
            {
                return null;
            }

            var sortableColumns = new List<PropertyNameTypeSort>();
            foreach (var prop in props)
            {
                object[] attributes = prop.GetCustomAttributes(true);
                foreach (object attribute in attributes)
                {
                    if (attribute is SortableAttribute sortableAttribute)
                    {
                        sortableColumns.Add(new PropertyNameTypeSort(sortableAttribute.SortName, prop.Name));
                    }
                }
            }

            foreach (var sort in clientSort)
            {
                if (!sortableColumns.Any(x => x.SortName == sort.SortName))
                {
                    throw new ArgumentException($"Sort not supported: {sort.SortName}");
                }

                foreach (var column in sortableColumns)
                {
                    if (sort.SortName == column.SortName)
                    {
                        if (!Enum.TryParse(sort.SortOrder, true, out SortOrder order))
                        {
                            throw new ArgumentException($"Sort not supported: {sort.SortOrder}");
                        }

                        result.Add(new AttributeSortOrder(order, column.PropertyName));
                    }
                }
            }

            return result;
        }

        private sealed class PropertyNameTypeSort
        {
            /// <summary>
            /// Parametrized constructor
            /// </summary>
            /// <param name="sortName">Sort name</param>
            /// <param name="propertyName">PropertyName</param>
            public PropertyNameTypeSort(string sortName, string propertyName)
            {
                SortName = sortName;
                PropertyName = propertyName;
            }

            public string SortName { get; set; }
            public string PropertyName { get; set; }
        }
    }
}
