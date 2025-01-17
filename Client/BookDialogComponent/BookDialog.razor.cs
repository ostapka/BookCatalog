using BookCatalog.Client.Interfaces;
using BookCatalog.Shared.Contracts.Response;
using BookCatalog.Shared.Request.Books;
using Microsoft.AspNetCore.Components;

namespace BookCatalog.Client.BookDialogComponent
{
    public partial class BookDialog : ComponentBase
    {
        [Inject] public IBookService BooksData { get; set; }
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }
        [Parameter] public BookResponse? Book { get; set; }
        [Parameter] public EventCallback<BookResponse?> BookChanged { get; set; }
        [Parameter] public EventCallback OnSave { get; set; }
        [Parameter] public bool adding { get; set; }

        private Task CloseModal()
        {
            IsVisible = false;
            return IsVisibleChanged.InvokeAsync(IsVisible);
        }

        private async Task SaveChanges()
        {
            if (Book != null && adding)
            {
                var book = new BookRequest()
                {
                    Title = Book.Title,
                    Author = Book.Author,
                    Genre = Book.Genre,
                    PublishedDate = Book.PublishedDate
                };
                await BooksData.AddBookAsync(book);
            }

            if (Book != null && !adding)
            {
                var book = new BookRequest()
                {
                    Title = Book.Title,
                    Author = Book.Author,
                    Genre = Book.Genre,
                    PublishedDate = Book.PublishedDate
                };
                await BooksData.UpdateBookAsync(Book.BookKey, book);
            }

            if (OnSave.HasDelegate)
            {
                await OnSave.InvokeAsync();
            }

            await CloseModal();
        }

        private void OnBookChanged()
        {
            if (BookChanged.HasDelegate)
            {
                BookChanged.InvokeAsync(Book);
            }
        }
    }
}
