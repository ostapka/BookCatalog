namespace BookCatalog.Shared.Contracts.Response
{
    /// <summary>
    /// Wrapper for holding collection responses
    /// </summary>
    /// <typeparam name="T">Type of the collection entities</typeparam>
    public class CollectionResponse<T>
    {
        /// <summary>
        /// Total count of item in the data store
        /// </summary>
        public int TotalCount { get; set; }



        /// <summary>
        /// List of items returned
        /// </summary>
        public IEnumerable<T> Data { get; set; }
    }
}
