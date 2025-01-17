﻿using AutoMapper;
using BookCatalog.Server.AppCore.Books.Commands;
using BookCatalog.Server.AppCore.Books.Commands.Handlers;
using BookCatalog.Server.Domain.Entities;
using BookCatalog.Shared.Interfaces.Repositories;
using BookCatalog.Shared.Models;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Net.Mime;
using System.Text;

namespace BookCatalog.Server.AppCore.Tests.Books.Handlers
{
    [TestClass]
    public class AddBooksCommandHandlerTests
    {
        private readonly Mock<IMapper> mapperMock = new Mock<IMapper>();
        private readonly Mock<IBookRepository> bookRepositoryMock = new Mock<IBookRepository>();
        private readonly Mock<IFormFile> formFile = new Mock<IFormFile>();
        private readonly AddBooksCommandHandler addBooksCommandHandler;

        public AddBooksCommandHandlerTests()
        {
            addBooksCommandHandler = new AddBooksCommandHandler(
                bookRepositoryMock.Object,
                mapperMock.Object);
        }

        [TestMethod]
        public async Task Handle_WhenAddBooksCommandIsValid_ShouldAddNewBooks()
        {
            // Arrange
            var command = new AddBooksCommand();
            var content = "[{\"Title\":\"test\"}]";
            using var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            // Mock the properties
            formFile.Setup(f => f.FileName).Returns("fileName");
            formFile.Setup(f => f.ContentType).Returns("multipart/form-data");
            formFile.Setup(f => f.Length).Returns(contentStream.Length);
            formFile.Setup(f => f.OpenReadStream()).Returns(contentStream);
            formFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                        .Callback<Stream, CancellationToken>((stream, token) =>
                        {
                            contentStream.Position = 0; // Reset position before copying
                            contentStream.CopyTo(stream);
                        })
                        .Returns(Task.CompletedTask);
            command.File = formFile.Object;

            var booksList = new List<Book> { new Book { Title = "test" } };
            var booksDtoList = new List<BookDto> { new BookDto { Title = "test" } };

            mapperMock
                .Setup(x => x.Map<IEnumerable<BookDto>>(booksList))
                .Returns(booksDtoList);

            bookRepositoryMock
                .Setup(x => x.AddBooksAsync(It.IsAny<IEnumerable<Book>>()))
                .ReturnsAsync(booksList);

            // Act
            var result = await addBooksCommandHandler.Handle(command, new CancellationToken());

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Handle_WhenAddBooksCommandIsNotValid_ShouldThrowException()
        {
            // Arrange
            var command = new AddBooksCommand();
            command.File = formFile.Object;

            // Act
            await addBooksCommandHandler.Handle(command, new CancellationToken());
            // Assert
        }
    }
}
