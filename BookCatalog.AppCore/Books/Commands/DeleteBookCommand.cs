using MediatR;

namespace BookCatalog.Server.AppCore.Books.Commands
{
    public class DeleteBookCommand : IRequest<bool>
    {
        public Guid BookKey { get; set; }

        public DeleteBookCommand(Guid bookKey)
        {
            BookKey = bookKey;
        }
    }
}
