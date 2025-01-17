namespace BookCatalog.Shared.Request.Sorting
{
    public interface ISortableRequest
    {
        public ClientSort[] Sort { get; set; }
    }
}
