using BookCatalog.Client.Interfaces;
using BookCatalog.Shared.Contracts.Response;
using BookCatalog.Shared.Request.Books;
using System.Net.Http.Json;
using System.Text;

namespace BookCatalog.Client.Services
{
    public class BookService : IBookService
    {
        private readonly HttpClient httpClient;

        public BookService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<CollectionResponse<BookResponse>> GetBooksAsync(int pageNumber, int pageSize, Dictionary<string, string> sort = null, string searchFactor = null)
        {
            var queruParam = GetParametrizedQuery(pageNumber, pageSize, sort, searchFactor);
            return await httpClient.GetFromJsonAsync<CollectionResponse<BookResponse>>($"api/book?{queruParam}") ?? new CollectionResponse<BookResponse>();
        }

        public async Task AddBookAsync(BookRequest book)
        {
            await httpClient.PostAsJsonAsync("api/book", book);
        }

        public async Task<HttpResponseMessage> AddBooksAsync(MultipartFormDataContent content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"api/book/bulkadd")
            {
                Content = content,
            };

            request.Headers.Add("accept", "application/json");

            return await httpClient.SendAsync(request);
        }

        public async Task UpdateBookAsync(Guid bookKey, BookRequest book)
        {
            await httpClient.PutAsJsonAsync($"api/book/{bookKey}", book);
        }

        public async Task DeleteBookAsync(Guid bookKey)
        {
            await httpClient.DeleteAsync($"api/book/{bookKey}");
        }

        private string GetParametrizedQuery(int? pageNumber, int? pageSize, Dictionary<string, string> sortParam, string searchFactor)
        {
            StringBuilder strB = new StringBuilder();

            if (pageNumber != null)
            {
                strB.Append($"offset={pageNumber - 1}&");
            }

            if (pageSize != null)
            {
                strB.Append($"limit={pageSize}&");
            }

            if (sortParam != null)
            {
                strB.Append($"sort.{sortParam["sortField"]}={sortParam["sortOrder"]}&");
            }

            if (!String.IsNullOrEmpty(searchFactor))
            {
                strB.Append($"search={searchFactor}");
            }
            return strB.ToString();
        }
    }
}
