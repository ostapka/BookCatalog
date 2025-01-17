namespace BookCatalog.Shared.Models
{
    /// <summary>
    /// Domain object for holding sortName and its order
    /// </summary>
    public class ClientSortDto
    {
        public string SortName { get; set; }
        public string SortOrder { get; set; }
    }
}
