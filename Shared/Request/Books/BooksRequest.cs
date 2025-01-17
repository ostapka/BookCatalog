using Microsoft.AspNetCore.Http;

namespace BookCatalog.Shared.Request.Books
{
    public class BooksRequest
    {
        public IFormFile File { get; set; }
    }
}
