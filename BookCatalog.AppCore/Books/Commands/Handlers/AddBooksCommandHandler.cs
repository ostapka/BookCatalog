using AutoMapper;
using BookCatalog.Server.Domain.Entities;
using BookCatalog.Shared.Interfaces.Repositories;
using BookCatalog.Shared.Models;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AddBooksCommandHandler> logger;

        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// AddBooksCommandHandler constructor
        /// </summary>
        /// <param name="bookRepository">Book repository instance</param>
        /// <param name="mapper">Mapper instace</param>
        public AddBooksCommandHandler(
            IBookRepository bookRepository, IMapper mapper, ILogger<AddBooksCommandHandler> logger)
        {
            this.bookRepository = bookRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<IEnumerable<BookDto>> Handle(AddBooksCommand command, CancellationToken cancellationToken)
        {
            if (command.File is null || command.File.Length == 0)
            {
                throw new ArgumentNullException(nameof(command), "command.File can't be null");
            }

            try
            {
                // Process the file
                using var streamReader = new StreamReader(command.File.OpenReadStream());
                var content = await streamReader.ReadToEndAsync(cancellationToken);

                var books = JsonSerializer.Deserialize<IEnumerable<Book>>(content, jsonOptions);

                if (books is not null)
                {
                    var response = await bookRepository.AddBooksAsync(books);

                    return mapper.Map<IEnumerable<BookDto>>(response);
                }

                throw new InvalidOperationException("The books object is in an invalid state.");
            }
            catch (JsonException jsonEx)
            {
                // Handle JSON deserialization errors
                logger.LogError(jsonEx, "An error occurred while deserializing the JSON content.");
                throw new InvalidOperationException("The provided file contains invalid data.", jsonEx);
            }
            catch (IOException ioEx)
            {
                // Handle file I/O errors
                logger.LogError(ioEx, "An error occurred while accessing the file stream.");
                throw new InvalidOperationException("There was a problem reading the file. Please try again.", ioEx);
            }
            catch (Exception ex)
            {
                // General exception handling
                logger.LogError(ex, "An unexpected error occurred while processing the request.");
                throw new InvalidOperationException("An error occurred while processing the file. Please contact support if the issue persists.", ex);
            }
        }
    }
}
