using BookCatalog.Server.Domain.Entities;
using BookCatalog.Server.Infrastructure.Data;
using BookCatalog.Shared.Interfaces.Common;
using BookCatalog.Shared.Interfaces.Repositories;
using BookCatalog.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace BookCatalog.Server.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext context;

        public BookRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public async Task<IPagedEnumerable<Book>> GetAsync(
            int limit,
            int offset,
            IEnumerable<AttributeSortOrder> sort = null,
            string search = null
        )
        {
            if (limit <= 0)
            {
                throw new ArgumentException($"Not Zero Error {limit}");
            }

            if (offset < 0)
            {
                throw new ArgumentException($"Not Zero Error {offset}");
            }

            if (sort is null)
            {
                sort = new List<AttributeSortOrder>
                {
                    new AttributeSortOrder(SortOrder.Desc, "Genre")
                };
            }

            var parameters = new Dictionary<string, object>();

            var count = context.Books.Count();

            if (count <= offset && count > 0)
            {
                throw new ArgumentException("Incorrect Offset Error");
            }

            parameters.Add("Offset", offset);
            parameters.Add("Limit", limit);

            var books1 = sort.Select(x => x.AttributeName);

            string orderBy = string.Join(", ", sort.Select(c => $"{c.AttributeName} {(c.SortOrder == SortOrder.Desc ? "desc" : "asc")}"));

            var books = context.Books.
                Where(book => !string.IsNullOrEmpty(search) ?
                    book.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    book.Author.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    book.Genre.Contains(search, StringComparison.OrdinalIgnoreCase) :
                    book == book).
                OrderBy(orderBy).
                Skip(offset * limit).
                Take(limit).
                ToList();

            var result = new PagedEnumerable<Book>(books, count);

            return result;
        }

        /// <inheritdoc/>
        public async Task<Book> GetByKeyAsync(Guid bookKey)
        {
            var book = await context.Books.FirstOrDefaultAsync(b => b.BookKey == bookKey);

            if (book == null)
            {
                throw new NullReferenceException();
            }

            return book;
        }

        /// <inheritdoc/>
        public async Task<Guid> AddAsync(Book book)
        {
            if (book.BookKey == Guid.Empty)
            {
                book.BookKey = Guid.NewGuid();
            }

            await context.Books.AddAsync(book);

            await context.SaveChangesAsync();

            return book.BookKey;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateAsync(Book book)
        {
            var updatingBook = await GetByKeyAsync(book.BookKey);

            if (updatingBook != null)
            {
                updatingBook.Title = book.Title;
                updatingBook.Author = book.Author;
                updatingBook.Genre = book.Genre;
                updatingBook.PublishedDate = book.PublishedDate;

                context.Books.Update(updatingBook);

                var state = context.Entry(updatingBook).State;

                await context.SaveChangesAsync();

                return state == EntityState.Modified;
            }

            return false;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(Guid bookKey)
        {
            var deletingBook = await GetByKeyAsync(bookKey);

            if (deletingBook != null)
            {
                context.Books.Remove(deletingBook);

                var state = context.Entry(deletingBook).State;

                await context.SaveChangesAsync();

                return state == EntityState.Deleted;
            }

            return false;
        }

        // <inheritdoc/>
        public async Task<IEnumerable<Book>> AddBooksAsync(IEnumerable<Book> books)
        {
            books = books.Select(b => new Book
            {
                BookKey = Guid.NewGuid(),
                Title = b.Title,
                Author = b.Author,
                Genre = b.Genre,
                PublishedDate = b.PublishedDate
            });

            await context.Books.AddRangeAsync(books);

            await context.SaveChangesAsync();

            return books;
        }
    }
}
