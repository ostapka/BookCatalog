using BookCatalog.Server.Domain.Attributes;

namespace BookCatalog.Server.Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public Guid BookKey { get; set; }

        [Sortable(nameof(Title))]
        public string Title { get; set; } = string.Empty;

        [Sortable(nameof(Author))]
        public string Author { get; set; } = string.Empty;
        [Sortable(nameof(Genre))]
        public string Genre { get; set; } = string.Empty;
        [Sortable(nameof(PublishedDate))]
        public DateTime PublishedDate { get; set; }
    }
}
