using BookCatalog.Shared.Contracts.Response;
using BookCatalog.Shared.Request.Books;

namespace BookCatalog.Client.Interfaces
{
    public interface IBookService
    {
        Task<CollectionResponse<BookResponse>> GetBooksAsync(
            int pageNumber, 
            int pageSize, 
            Dictionary<string, string> sort = null, 
            string searchFactor = null);

        Task AddBookAsync(BookRequest book);

        Task<HttpResponseMessage> AddBooksAsync(MultipartFormDataContent content);

        Task UpdateBookAsync(Guid bookKey, BookRequest book);

        Task DeleteBookAsync(Guid bookKey);
    }
}
