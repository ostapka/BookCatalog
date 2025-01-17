using BookCatalog.Server.Domain.Entities;
using BookCatalog.Server.Infrastructure.Data;
using BookCatalog.Server.Infrastructure.Repositories;
using BookCatalog.Shared.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using BookCatalog.Shared.Models;

namespace BookCatalog.Server.Infrastrurture.Tests.Repositories
{
    [TestClass]
    public class BookRepositoryTests
    {
        private readonly Mock<DbSet<Book>> mockDbSet = new Mock<DbSet<Book>>();
        private readonly Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();
        private readonly IBookRepository repository;

        public BookRepositoryTests()
        {
            repository = new BookRepository(mockContext.Object);
        }

        [TestMethod]
        public async Task GetAsync_GetBooks()
        {
            // Arrange
            var limit = 100;
            var offset = 0;
            var sort = new List<AttributeSortOrder>();
            var search = "test";
            var booksList = new List<Book>() { new Book { BookKey = Guid.NewGuid(), Title = "test"} };

            mockContext.Setup(m => m.Books.Count()).Returns(5);
            mockContext.Setup(m => m.Books.
            Where(x => true).
            OrderBy(It.IsAny<string>()).
            Skip(It.IsAny<int>()).
            Take(It.IsAny<int>()).
            ToList()).Returns(booksList);

            // Act
            var result = await repository.GetAsync(limit, offset, sort, search);

            // Assert
            Assert.IsNotNull(result);
        }

    }
}
