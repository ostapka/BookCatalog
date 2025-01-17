using AutoMapper;
using BookCatalog.Server.Domain.Entities;
using BookCatalog.Shared.Interfaces.Repositories;
using BookCatalog.Shared.Models;
using MediatR;
using System.Text.Json;

namespace BookCatalog.Server.AppCore.Books.Commands.Handlers
{
    /// <summary>
    /// Handler responsible for creating new book
    /// </summary>
    public sealed class AddBooksCommandHandler : IRequestHandler<AddBooksCommand, IEnumerable<BookDto>>
    {
        private readonly IBookRepository bookRepository;
        private readonly IMapper mapper;

        /// <summary>
        /// AddBooksCommandHandler constructor
        /// </summary>
        /// <param name="bookRepository">Book repository instance</param>
        /// <param name="mapper">Mapper instace</param>
        public AddBooksCommandHandler(
            IBookRepository bookRepository, IMapper mapper)
        {
            this.bookRepository = bookRepository;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<BookDto>> Handle(AddBooksCommand command, CancellationToken cancellationToken)
        {
            if (command.File == null || command.File.Length == 0)
            {
                throw new ArgumentNullException();
            }

            try
            {
                // Process the file
                using var streamReader = new StreamReader(command.File.OpenReadStream());
                var content = await streamReader.ReadToEndAsync();

                var books = JsonSerializer.Deserialize<IEnumerable<Book>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var response = await bookRepository.AddBooksAsync(books);

                return mapper.Map<IEnumerable<BookDto>>(response);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
