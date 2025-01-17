namespace BookCatalog.Shared.Request.Sorting
{
    /// <summary>
    /// Object for holding sortField and its order
    /// </summary>
    public class ClientSort
    {
        public string SortName { get; set; }
        public string SortOrder { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sortName">Field name to sort</param>
        /// <param name="sortOrder">Sort order</param>
        public ClientSort(string sortName, string sortOrder)
        {
            SortName = sortName;
            SortOrder = sortOrder;
        }
    }
}
