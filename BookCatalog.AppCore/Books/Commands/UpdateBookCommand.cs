namespace BookCatalog.Server.AppCore.Books.Commands
{
    public class UpdateBookCommand : CreateBookCommand
    {
        public Guid BookKey { get; set; }
    }
}
