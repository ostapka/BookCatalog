using AutoMapper;
using BookCatalog.Server.AppCore.Exceptions;
using BookCatalog.Shared.Interfaces.Repositories;
using BookCatalog.Shared.Models;
using MediatR;
using Error = BookCatalog.Server.AppCore.Exceptions.ErrorCode;

namespace BookCatalog.Server.AppCore.Books.Queries.Handlers
{
    /// <summary>
    /// Query handler for GetBookByKeyQuery
    /// </summary>
    public class GetBookByKeyQueryHandler : IRequestHandler<GetBookByKeyQuery, BookDto>
    {
        private readonly IBookRepository bookRepository;
        private readonly IMapper mapper;

        /// <summary>
        /// GetBookByKeyQueryHandler constructor
        /// </summary>
        /// <param name="bookRepository">Book repository instance</param>
        /// <param name="mapper">Mapper instance</param>
        public GetBookByKeyQueryHandler(IBookRepository bookRepository, IMapper mapper)
        {
            this.bookRepository = bookRepository;
            this.mapper = mapper;
        }

        public async Task<BookDto> Handle(GetBookByKeyQuery query, CancellationToken cancellationToken)
        {
            var book = await bookRepository.GetByKeyAsync(query.BookKey);

            if (book is null)
            {
                throw new NotFoundException($"Could not find {nameof(book)}", Error.NotFound);
            }

            return mapper.Map<BookDto>(book);
        }
    }
}
