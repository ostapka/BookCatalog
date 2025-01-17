using BookCatalog.Shared.Interfaces.Common;
using BookCatalog.Shared.Models;
using MediatR;

namespace BookCatalog.Server.AppCore.Books.Queries
{
    public class GetBooksPageQuery : IRequest<IPagedEnumerable<BookDto>>
    {
        public int Limit { get; set; }

        public int Offset { get; set; }

        public IEnumerable<ClientSortDto> Sort { get; set; }

        public string Search { get; set; }

        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="limit">Amount of Books to return, greater than 0</param>
        /// <param name="offset">Offset, >= 0</param>
        /// <param name="sort">Sort options of the query</param>
        public GetBooksPageQuery(
            int limit,
            int offset,
            IEnumerable<ClientSortDto> sort = null,
            string search = null
        )
        {
            Limit = limit;
            Offset = offset;
            Sort = sort;
            Search = search;
        }
    }
}
