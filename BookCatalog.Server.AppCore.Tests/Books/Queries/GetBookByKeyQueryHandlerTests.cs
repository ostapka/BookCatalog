using AutoMapper;
using BookCatalog.Server.AppCore.Books.Queries;
using BookCatalog.Server.AppCore.Books.Queries.Handlers;
using BookCatalog.Server.AppCore.Exceptions;
using BookCatalog.Server.Domain.Entities;
using BookCatalog.Shared.Interfaces.Repositories;
using BookCatalog.Shared.Models;
using Moq;

namespace BookCatalog.Server.AppCore.Tests.Books.Queries
{
    [TestClass]
    public class GetBookByKeyQueryHandlerTests
    {
        private readonly Mock<IBookRepository> bookRepositoryMock = new Mock<IBookRepository>();
        private readonly Mock<IMapper> mapperMock = new Mock<IMapper>();
        private readonly GetBookByKeyQueryHandler getBookByKeyQueryHandler;

        public GetBookByKeyQueryHandlerTests()
        {
            getBookByKeyQueryHandler = new GetBookByKeyQueryHandler(
                bookRepositoryMock.Object,
                mapperMock.Object
                );
        }

        [TestMethod]
        public async Task Handle_WhenBookExists_ShouldReturnBook()
        {
            // Arrange
            var bookKey = Guid.NewGuid();
            var query = new GetBookByKeyQuery(bookKey);
            var book = new Book { BookKey = query.BookKey };
            var bookDto = new BookDto { BookKey = book.BookKey };

            bookRepositoryMock
                .Setup(x => x.GetByKeyAsync(query.BookKey))
                .ReturnsAsync(book);
            mapperMock
                .Setup(x => x.Map<BookDto>(book))
                .Returns(bookDto);

            // Act
            var result = await getBookByKeyQueryHandler.Handle(query, new CancellationToken());

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Handle_WhenBookDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var bookKey = Guid.NewGuid();
            var query = new GetBookByKeyQuery(bookKey);

            // Act
            // Assert

            var result = await Assert.ThrowsExceptionAsync<NotFoundException>(
                async () => await getBookByKeyQueryHandler.Handle(query, new CancellationToken())
            );
        }
    }
}
