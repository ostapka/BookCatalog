using System.ComponentModel.DataAnnotations;

namespace BookCatalog.Shared.Request.Books
{
    public class BookRequest
    {
        [Required]
        [MaxLength(255)]
        public required string Title { get; set; }
        [MaxLength(255)]
        public string Author { get; set; } = string.Empty;
        [MaxLength(255)]
        public string Genre { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
    }
}
