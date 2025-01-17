namespace BookCatalog.Server.Domain.Attributes
{
    /// <summary>
    /// Attribute for marking fields of entity as sortable
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SortableAttribute : Attribute
    {
        /// <summary>
        /// name of the sort property that is passed by the client
        /// </summary>
        public string SortName { get; }

        /// <summary>
        /// Attribute constructor
        /// </summary>
        /// <param name="sortName">name of the sort property that is passed by the client</param>
        public SortableAttribute(string sortName)
        {
            SortName = sortName.ToUpperInvariant();
        }
    }
}
