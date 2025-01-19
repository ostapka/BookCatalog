using BookCatalog.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BookCatalog.Server.AppCore.Books.Commands
{
    public class AddBooksCommand : IRequest<IEnumerable<BookDto>>
    {
        public required IFormFile File { get; set; }
    }
}
