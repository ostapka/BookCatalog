using BookCatalog.Server.Domain.Entities;
using BookCatalog.Shared.Interfaces.Common;
using BookCatalog.Shared.Models;

namespace BookCatalog.Shared.Interfaces.Repositories
{
    public interface IBookRepository
    {
        /// <summary>
        /// Method for books paging, sorting and searching
        /// </summary>
        /// <param name="limit">Amount of Books to return, greater than 0</param>
        /// <param name="offset">offset, greater than 0</param>
        /// <param name="sort">List of AttributeDirectionValue instances used for sorting</param>
        /// <param name="search">Search factor</param>
        /// <returns>page data and count of Books</returns>
        Task<IPagedEnumerable<Book>> GetAsync(
            int limit,
            int offset,
            IEnumerable<AttributeSortOrder> sort = null,
            string search = null
        );

        /// <summary>
        /// Returns book with specific key
        /// </summary>
        /// <param name="bookKey">The key of the book to look for</param>
        /// <returns>Book found</returns>
        Task<Book> GetByKeyAsync(Guid bookKey);

        /// <summary>
        /// Create new book
        /// </summary>
        /// <param name="book">Book to create</param>
        /// <returns>Key of created book</returns>
        Task<Guid> AddAsync(Book book);

        /// <summary>
        /// Add books from csv file
        /// </summary>
        /// <param name="books">Books to add</param>
        /// <returns>IEnumerable of created books</returns>
        Task<IEnumerable<Book>> AddBooksAsync(IEnumerable<Book> books);

        /// <summary>
        /// Update book
        /// </summary>
        /// <param name="book">Book to create</param>
        /// <returns>True if the book has been updated</returns>
        Task<bool> UpdateAsync(Book book);

        /// <summary>
        /// Delete book
        /// </summary>
        /// <param name="book">Book to create</param>
        /// <returns>True if the book has been deleted</returns>
        Task<bool> DeleteAsync(Guid bookKey);
    }
}
