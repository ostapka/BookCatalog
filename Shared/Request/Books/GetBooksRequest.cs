using BookCatalog.Shared.Request.Sorting;
using System.ComponentModel.DataAnnotations;

namespace BookCatalog.Shared.Request.Books
{
    public class GetBooksRequest : ISortableRequest
    {
        [Range(1, 1000)]
        public int Limit { get; set; } = 5;

        [Range(0, int.MaxValue)]
        public int Offset { get; set; } = 0;

        public ClientSort[]? Sort { get; set; }

        public string? Search { get; set; }
    }
}
