namespace BookCatalog.Shared.Models
{
    public class BookDto
    {
        public Guid BookKey { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public DateTime PublishedDate { get; set; }
    }
}
