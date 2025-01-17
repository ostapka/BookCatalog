using AutoMapper;
using BookCatalog.Server.AppCore.Books.Queries;
using BookCatalog.Server.AppCore.Books.Queries.Handlers;
using BookCatalog.Server.Domain.Entities;
using BookCatalog.Shared.Interfaces.Repositories;
using BookCatalog.Shared.Models;
using Moq;

namespace BookCatalog.Server.AppCore.Tests.Books.Queries
{
    [TestClass]
    public class GetBooksPageQueryHandlerTests
    {
        private readonly Mock<IBookRepository> bookRepositoryMock = new Mock<IBookRepository>();
        private readonly Mock<IMapper> mapperMock = new Mock<IMapper>();
        private readonly GetBooksPageQueryHandler getBooksPageQueryHandler;

        public GetBooksPageQueryHandlerTests()
        {
            getBooksPageQueryHandler = new GetBooksPageQueryHandler(mapperMock.Object, bookRepositoryMock.Object);
        }

        [TestMethod]
        public async Task Handle_WhenBookExists_ShouldReturnBook()
        {
            // Arrange
            var query = new GetBooksPageQuery(10, 10);
            var book = new Book { BookKey = Guid.NewGuid() };
            var bookDto = new BookDto { BookKey = book.BookKey };
            var pagedBookResponse = new PagedEnumerable<Book>(new List<Book>() { book }, 10);

            bookRepositoryMock
                .Setup(x => x.GetAsync(
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<List<AttributeSortOrder>>(),
                        It.IsAny<string>()))
                .ReturnsAsync(pagedBookResponse);

            mapperMock
                .Setup(x => x.Map<BookDto>(book))
                .Returns(bookDto);

            // Act
            var result = await getBooksPageQueryHandler.Handle(query, new CancellationToken());

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
