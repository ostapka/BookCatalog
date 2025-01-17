using AutoMapper;
using BookCatalog.Server.AppCore.Books.Commands;
using BookCatalog.Server.AppCore.Books.Queries;
using BookCatalog.Server.Controllers;
using BookCatalog.Server.Infrastructure;
using BookCatalog.Shared.Contracts.Response;
using BookCatalog.Shared.Models;
using BookCatalog.Shared.Request.Books;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System.Security.Cryptography.X509Certificates;


/// <summary>
/// Books tests controller
/// </summary>
namespace BookCatalog.Server.Tests.Controllers
{
    [TestClass]
    public class BookControllerTests
    {
        private readonly Mock<IMediator> mediatorMock = new Mock<IMediator>();
        private readonly Mock<IMapper> mapperMock = new Mock<IMapper>();
        private readonly Mock<IHubContext<BookHub>> hubContextMock = new Mock<IHubContext<BookHub>>();
        private readonly Mock<IHubClients> mockClients = new Mock<IHubClients>();
        private readonly Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
        private readonly Mock<IFormFile> formFile = new Mock<IFormFile>();
        private readonly BookController bookController;

        public BookControllerTests()
        {
            bookController = new BookController(mediatorMock.Object, mapperMock.Object, hubContextMock.Object);
        }

        /// <summary>
        /// Gets the by identifier when book exists should return200 status code with book.
        /// </summary>
        [TestMethod]
        public async Task GetByKey_WhenBookExists_ShouldReturn200StatusCodeWithBook()
        {
            // Arrange
            var bookKey = Guid.NewGuid();
            var bookDto = new BookDto { BookKey = bookKey };
            var bookResponse = new BookResponse { BookKey = bookKey };

            mediatorMock
                .Setup(x => x.Send(It.IsAny<GetBookByKeyQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookDto);
            mapperMock
                .Setup(x => x.Map<BookResponse>(bookDto))
                .Returns(bookResponse);

            // Act
            var result = await bookController.GetBookByKey(bookKey);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.IsNotNull((result.Result as OkObjectResult).Value);
        }

        /// <summary>
        /// Creates. When request is valid should create book.
        /// </summary>
        [TestMethod]
        public async Task Create_WhenRequestIsValid_ShouldCreateBook()
        {
            // Arrange
            var request = new BookRequest
            {
                Title = "",
                Author = "",
            };
            var command = new CreateBookCommand
            {
                Title = request.Title,
                Author = request.Author
            };
            var bookDto = new BookDto
            {
                BookKey = Guid.NewGuid(),
                Title = command.Title,
                Author = command.Author
            };
            var bookResponse = new BookResponse
            {
                BookKey = bookDto.BookKey,
                Title = bookDto.Title,
                Author = bookDto.Author
            };

            mapperMock
                .Setup(x => x.Map<CreateBookCommand>(request))
                .Returns(command);
            mediatorMock
                .Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookDto);
            mapperMock
                .Setup(x => x.Map<BookResponse>(bookDto))
                .Returns(bookResponse);
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            hubContextMock.Setup(hub => hub.Clients).Returns(mockClients.Object);

            // Act
            var result = await bookController.CreateBook(request);

            // Assert
            mockClientProxy.Verify(
                client => client.SendCoreAsync(
                    "ReceiveBookUpdate",
                    It.Is<object[]>(args => args.Length == 1 && (string)args[0] == "A new book was added."),
                    default),
                Times.Once
            );
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            Assert.IsNotNull((result as CreatedAtActionResult).Value);
        }

        [TestMethod]
        public async Task GetPage_ShouldReturnBooks_WhenBooksExist()
        {
            // Arrange
            PagedEnumerable<BookDto> books = new PagedEnumerable<BookDto>(new List<BookDto>()
                {
                    new BookDto()
                    {
                        BookKey = Guid.NewGuid(),
                        Title = "test1",
                        Author = "Author_1"
                    },
                    new BookDto()
                    {
                        BookKey = Guid.NewGuid(),
                        Title = "test2",
                        Author = "Author_2"
                    }
                }, 10);

            mediatorMock
               .Setup(d => d.Send(It.IsAny<GetBooksPageQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(books);

            // Act
            var result = await bookController.GetPage(new GetBooksRequest()
            {
                Offset = 0,
                Limit = 10
            });

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(typeof(OkObjectResult).IsAssignableFrom(result.Result.GetType()));
        }

        /// <summary>
        /// Updates the book asynchronous should update book when book exists.
        /// </summary>
        [TestMethod]
        public async Task UpdateBookAsync_ShouldUpdateBook_WhenBookExists()
        {
            // Arrange
            var bookKey = Guid.NewGuid();

            var bookRequest = new BookRequest()
            {
                Title = "test",
                Author = "Author_1"
            };

            var command = new UpdateBookCommand
            {
                BookKey = bookKey,
                Title = bookRequest.Title,
                Author = bookRequest.Author
            };

            var response = new BookDto()
            {
                BookKey = bookKey,
                Title = "test",
                Author = "Author_1"
            };

            mapperMock
                .Setup(x => x.Map<UpdateBookCommand>(bookRequest))
                .Returns(command);

            mediatorMock
               .Setup(d => d.Send(command, It.IsAny<CancellationToken>()))
               .ReturnsAsync(response);

            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            hubContextMock.Setup(hub => hub.Clients).Returns(mockClients.Object);

            // Act
            var result = await bookController.PutBook(bookKey, bookRequest);

            // Assert
            mockClientProxy.Verify(
                client => client.SendCoreAsync(
                    "ReceiveBookUpdate",
                    It.Is<object[]>(args => args.Length == 1 && (string)args[0] == "A book was updated."),
                    default),
                Times.Once
            );
            Assert.IsNotNull(result);
            Assert.IsTrue(typeof(OkObjectResult).IsAssignableFrom(result.Result.GetType()));
        }

        /// <summary>
        /// Deletes the book asynchronous should delete book when book exists.
        /// </summary>
        [TestMethod]
        public async Task DeleteBookAsync_ShouldDeleteBook_WhenBookExists()
        {
            // Arrange
            var bookKey = Guid.NewGuid();

            var command = new DeleteBookCommand(bookKey);

            mapperMock
                .Setup(x => x.Map<DeleteBookCommand>(bookKey))
                .Returns(command);

            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            hubContextMock.Setup(hub => hub.Clients).Returns(mockClients.Object);

            // Act
            var result = await bookController.DeleteBook(bookKey);

            // Assert
            mockClientProxy.Verify(
                client => client.SendCoreAsync(
                    "ReceiveBookUpdate",
                    It.Is<object[]>(args => args.Length == 1 && (string)args[0] == "A book was deleted."),
                    default),
                Times.Once
            );
            Assert.IsNotNull(result);
            Assert.IsTrue(typeof(NoContentResult).IsAssignableFrom(result.GetType()));
        }

        [TestMethod]
        public async Task AddBooksAsync_ShouldAddBooks()
        {
            // Arrange
            var request = new BooksRequest
            {
                File = formFile.Object
            };
            var command = new AddBooksCommand
            {
                File = request.File
            };
            var bookKey = Guid.NewGuid();
            var booksDtoList = new List<BookDto> { new BookDto { BookKey = bookKey, Title = "test" } };
            var booksResponseList = new List<BookResponse> { new BookResponse { BookKey = bookKey, Title = "test" } };

            mapperMock.
                Setup(x => x.Map<AddBooksCommand>(request)).
                Returns(command);
            mapperMock.
                Setup(x => x.Map<IEnumerable<BookResponse>>(booksDtoList)).
                Returns(booksResponseList);
            mediatorMock
                .Setup(x => x.Send(It.IsAny<AddBooksCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(booksDtoList);
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            hubContextMock.Setup(hub => hub.Clients).Returns(mockClients.Object);

            // Act
            var result = await bookController.AddBooks(request);

            // Assert
            mockClientProxy.Verify(
                client => client.SendCoreAsync(
                    "ReceiveBookUpdate",
                    It.Is<object[]>(args => args.Length == 1 && (string)args[0] == "New books were added."),
                    default),
                Times.Once
            );
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            Assert.IsNotNull((result as CreatedAtActionResult).Value);
        }
    }
}
