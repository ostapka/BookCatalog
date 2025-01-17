using AutoMapper;
using BookCatalog.Server.AppCore.Books.Commands;
using BookCatalog.Server.AppCore.Books.Commands.Handlers;
using BookCatalog.Server.Domain.Entities;
using BookCatalog.Shared.Interfaces.Repositories;
using BookCatalog.Shared.Models;
using Moq;

namespace BookCatalog.Server.AppCore.Tests.Books.Handlers
{
    [TestClass]
    public class UpdateBookCommandHandlerTests
    {
        private readonly Mock<IMapper> mapperMock = new Mock<IMapper>();
        private readonly Mock<IBookRepository> bookRepositoryMock = new Mock<IBookRepository>();
        private readonly UpdateBookCommandHandler updateBookCommandHandler;

        public UpdateBookCommandHandlerTests()
        {
            updateBookCommandHandler = new UpdateBookCommandHandler(
                bookRepositoryMock.Object,
                mapperMock.Object);
        }

        [TestMethod]
        public async Task Handle_WhenUpdateBookCommandIsValid_ShouldUpdateBook()
        {
            // Arrange
            var command = new UpdateBookCommand
            {
                BookKey = Guid.NewGuid(),
                Title = "Test",
                Author = "Test author",
                PublishedDate = new DateTime()
            };
            var book = new Book { BookKey = command.BookKey, Title = "Test", Author = "Test author", PublishedDate = new DateTime() };
            var bookDto = new BookDto
            {
                BookKey = book.BookKey,
                Title = book.Title,
                Author = book.Author,
                PublishedDate = book.PublishedDate
            };

            bookRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Book>()))
                .ReturnsAsync(true);

            mapperMock
                .Setup(x => x.Map<Book>(command))
                .Returns(book);
            mapperMock
                .Setup(x => x.Map<BookDto>(book))
                .Returns(bookDto);

            // Act
            var updatedBook = await updateBookCommandHandler.Handle(command, new CancellationToken());

            // Assert
            Assert.IsNotNull(updatedBook);
        }
    }
}
