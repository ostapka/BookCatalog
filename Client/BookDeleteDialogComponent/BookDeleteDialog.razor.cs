using BookCatalog.Client.Interfaces;
using BookCatalog.Shared.Contracts.Response;
using Microsoft.AspNetCore.Components;

namespace BookCatalog.Client.BookDeleteDialogComponent
{
    public partial class BookDeleteDialog : ComponentBase
    {
        [Inject] public IBookService BooksData { get; set; }

        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }
        [Parameter] public BookResponse? Book { get; set; }
        public string Title { get; set; } = "Delete Book";

        private Task CloseModal()
        {
            IsVisible = false;
            return IsVisibleChanged.InvokeAsync(IsVisible);
        }

        private async Task Delete()
        {
            if (Book != null)
            {

                await BooksData.DeleteBookAsync(Book.BookKey);
            }

            await CloseModal();
        }

    }
}
