namespace BookCatalog.Shared.Models
{
    /// <summary>
    /// Object that holds column from DB and SortDirection
    /// </summary>
    public class AttributeSortOrder
    {
        public string AttributeName { get; }
        public SortOrder SortOrder { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sortOrder"><see cref="SortOrder"/></param>
        /// <param name="attributeName">Name of the sortable column</param>
        public AttributeSortOrder(SortOrder sortOrder, string attributeName)
        {
            SortOrder = sortOrder;
            AttributeName = attributeName;
        }
    }
}
