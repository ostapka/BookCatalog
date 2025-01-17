namespace BookCatalog.Shared.Interfaces.Common
{
    /// <summary>
    /// Class for holding a subset of the enumerable entities (e.g. Page)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPagedEnumerable<out T> : IEnumerable<T>
    {
        /// <summary>
        /// Holds total amount of entities
        /// </summary>
        public int TotalCount { get; }

    }
}
