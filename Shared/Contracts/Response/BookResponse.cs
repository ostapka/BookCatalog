namespace BookCatalog.Shared.Contracts.Response
{
    public class BookResponse
    {
        public Guid BookKey { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
    }
}
