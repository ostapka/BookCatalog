namespace BookCatalog.Shared.Contracts.Response
{
    /// <summary>
    /// Wrapper for standard entity response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleEntityResponse<T>
    {
        /// <summary>
        /// parametrized ctor
        /// </summary>
        /// <param name="data"></param>
        public SingleEntityResponse(T data)
        {
            Data = data;
        }

        /// <summary>
        /// Object that will be returned
        /// </summary>
        public T Data { get; set; }
    }
}
