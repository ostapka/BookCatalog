using AutoMapper;
using BookCatalog.Server.Domain.Entities;
using BookCatalog.Shared.Interfaces.Common;
using BookCatalog.Shared.Interfaces.Repositories;
using BookCatalog.Shared.Models;
using MediatR;

namespace BookCatalog.Server.AppCore.Books.Queries.Handlers
{
    public class GetBooksPageQueryHandler : IRequestHandler<GetBooksPageQuery, IPagedEnumerable<BookDto>>
    {
        private readonly IBookRepository bookRepository;
        private readonly IMapper mapper;

        /// <summary>
        /// Parametrized contructor for Handler
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="bookRepository"><see cref="IBookRepository"></param>
        public GetBooksPageQueryHandler(IMapper mapper, IBookRepository bookRepository)
        {
            this.mapper = mapper;
            this.bookRepository = bookRepository;
        }

        public async Task<IPagedEnumerable<BookDto>> Handle(GetBooksPageQuery query, CancellationToken cancellationToken)
        {
            var sort = SortMapper.MapSort<Book>(query.Sort);
            var data = await bookRepository.GetAsync(query.Limit, query.Offset, sort, query.Search);
            var mappedData = mapper.Map<IEnumerable<Book>, IEnumerable<BookDto>>(data);
            var result = new PagedEnumerable<BookDto>(mappedData, data.TotalCount);
            return result;
        }
    }
}
