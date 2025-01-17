using BookCatalog.Shared.Models;
using MediatR;

namespace BookCatalog.Server.AppCore.Books.Queries
{
    public class GetBookByKeyQuery : IRequest<BookDto>
    {
        public GetBookByKeyQuery(Guid bookKey)
        {
            BookKey = bookKey;
        }

        public Guid BookKey { get; set; }
    }
}
