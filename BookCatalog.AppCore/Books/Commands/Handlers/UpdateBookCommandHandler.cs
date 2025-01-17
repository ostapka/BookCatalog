using AutoMapper;
using BookCatalog.Server.AppCore.Exceptions;
using BookCatalog.Server.Domain.Entities;
using BookCatalog.Shared.Interfaces.Repositories;
using BookCatalog.Shared.Models;
using MediatR;
using Error = BookCatalog.Server.AppCore.Exceptions.ErrorCode;

namespace BookCatalog.Server.AppCore.Books.Commands.Handlers
{
    public sealed class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, BookDto>
    {
        private readonly IBookRepository bookRepository;
        private readonly IMapper mapper;

        /// <summary>
        /// UpdateBookCommandHandler constructor
        /// </summary>
        /// <param name="bookRepository">Book repository instance</param>
        /// <param name="mapper">Mapper instace</param>
        public UpdateBookCommandHandler(
            IBookRepository bookRepository, IMapper mapper)
        {
            this.bookRepository = bookRepository;
            this.mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<BookDto> Handle(UpdateBookCommand command, CancellationToken cancellationToken)
        {
            var requestItem = mapper.Map<Book>(command);

            if (!await bookRepository.UpdateAsync(requestItem))
            {
                throw new NotFoundException($"Could not update a book with the key {requestItem.BookKey}", Error.NotFound);
            }

            return mapper.Map<BookDto>(requestItem);
        }
    }
}
