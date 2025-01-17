using BookCatalog.Shared.Models;
using MediatR;

namespace BookCatalog.Server.AppCore.Books.Commands
{
    public class CreateBookCommand : IRequest<BookDto>
    {
        public required string Title { get; set; }
        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
    }
}
