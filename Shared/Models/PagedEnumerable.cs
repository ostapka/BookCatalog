using BookCatalog.Shared.Interfaces.Common;
using System.Collections;

namespace BookCatalog.Shared.Models
{
    /// <summary>
    /// Class holds a subset of the collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedEnumerable<T> : IPagedEnumerable<T>
    {
        /// <summary>
        /// Holds total amount of entities
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// Holds page data
        /// </summary>
        private readonly IEnumerable<T> page;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="page">data itself</param>
        /// <param name="totalCount">Superset count</param>
        public PagedEnumerable(IEnumerable<T> page, int totalCount)
        {
            TotalCount = totalCount;
            this.page = page;
        }

        /// <summary>
        ///  Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return page.GetEnumerator();
        }

        /// <summary>
        ///  Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return page.GetEnumerator();
        }
    }
}
