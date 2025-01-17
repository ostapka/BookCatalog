using BookCatalog.Server.AppCore.Exceptions;
using BookCatalog.Shared.Interfaces.Repositories;
using MediatR;
using Error = BookCatalog.Server.AppCore.Exceptions.ErrorCode;

namespace BookCatalog.Server.AppCore.Books.Commands.Handlers
{
    /// <summary>
    /// Handler responsible for deleting book
    /// </summary>
    public sealed class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, bool>
    {
        private readonly IBookRepository bookRepository;

        /// <summary>
        /// DeleteBookCommandHandler constructor
        /// </summary>
        /// <param name="bookRepository">Book repository instance</param>
        /// <param name="mapper">Mapper instace</param>
        public DeleteBookCommandHandler(
            IBookRepository bookRepository)
        {
            this.bookRepository = bookRepository;
        }

        public async Task<bool> Handle(DeleteBookCommand command, CancellationToken cancellationToken)
        {
            var result = await bookRepository.DeleteAsync(command.BookKey);

            if (!result)
            {
                throw new NotFoundException($"Could not delete a book with the key {command.BookKey}", Error.NotFound);
            }

            return result;
        }
    }
}
