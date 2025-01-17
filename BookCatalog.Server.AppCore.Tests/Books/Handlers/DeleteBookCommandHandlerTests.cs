using BookCatalog.Server.AppCore.Books.Commands;
using BookCatalog.Server.AppCore.Books.Commands.Handlers;
using BookCatalog.Server.Domain.Entities;
using BookCatalog.Shared.Interfaces.Repositories;
using Moq;

namespace BookCatalog.Server.AppCore.Tests.Books.Handlers
{
    [TestClass]
    public class DeleteBookCommandHandlerTests
    {
        private readonly Mock<IBookRepository> bookRepositoryMock = new Mock<IBookRepository>();
        private readonly DeleteBookCommandHandler deleteBookCommandHandler;

        public DeleteBookCommandHandlerTests()
        {
            deleteBookCommandHandler = new DeleteBookCommandHandler(
                bookRepositoryMock.Object);
        }

        [TestMethod]
        public async Task Handle_WhenBookExists_ShouldDelete()
        {
            // Arrange
            var command = new DeleteBookCommand(Guid.NewGuid());
            var book = new Book { BookKey = command.BookKey, Title = "Test", Author = "Test author", PublishedDate = new DateTime() };

            bookRepositoryMock
                .Setup(x => x.DeleteAsync(command.BookKey))
                .ReturnsAsync(true);

            // Act
            var result = await deleteBookCommandHandler.Handle(command, new CancellationToken());

            // Assert
            Assert.IsTrue(result);
        }
    }
}
