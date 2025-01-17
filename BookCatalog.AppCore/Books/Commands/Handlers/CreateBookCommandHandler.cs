using AutoMapper;
using BookCatalog.Server.Domain.Entities;
using BookCatalog.Shared.Interfaces.Repositories;
using BookCatalog.Shared.Models;
using MediatR;

namespace BookCatalog.Server.AppCore.Books.Commands.Handlers
{
    /// <summary>
    /// Handler responsible for creating new book
    /// </summary>
    public sealed class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
    {
        private readonly IBookRepository bookRepository;
        private readonly IMapper mapper;

        /// <summary>
        /// CreateBookCommandHandler constructor
        /// </summary>
        /// <param name="bookRepository">Book repository instance</param>
        /// <param name="mapper">Mapper instace</param>
        public CreateBookCommandHandler(
            IBookRepository bookRepository, IMapper mapper)
        {
            this.bookRepository = bookRepository;
            this.mapper = mapper;
        }

        public async Task<BookDto> Handle(CreateBookCommand command, CancellationToken cancellationToken)
        {
            var requestItem = mapper.Map<Book>(command);

            await bookRepository.AddAsync(requestItem);

            return mapper.Map<BookDto>(requestItem);
        }
    }
}
