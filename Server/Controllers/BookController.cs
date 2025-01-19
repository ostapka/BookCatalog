using AutoMapper;
using BookCatalog.Server.AppCore.Books.Commands;
using BookCatalog.Server.AppCore.Books.Queries;
using BookCatalog.Server.Infrastructure;
using BookCatalog.Shared.Contracts.Response;
using BookCatalog.Shared.Models;
using BookCatalog.Shared.Request.Books;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BookCatalog.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        private readonly IHubContext<BookHub> hubContext;
        public BookController(IMediator mediator, IMapper mapper, IHubContext<BookHub> hubContext)
        {
            this.mediator = mediator;
            this.mapper = mapper;
            this.hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<CollectionResponse<BookResponse>>> GetPage(
            [FromQuery] GetBooksRequest request)
        {
            var clientSortDto = mapper.Map<IEnumerable<ClientSortDto>>(request.Sort);

            var data = await mediator.Send(new GetBooksPageQuery(request.Limit, request.Offset, clientSortDto, request.Search));
            var mappedData = mapper.Map<IEnumerable<BookDto>, IEnumerable<BookResponse>>(data);
            var result = new CollectionResponse<BookResponse>()
            {
                Data = mappedData,
                TotalCount = data.TotalCount
            };

            return Ok(result);
        }

        [HttpGet("{bookKey}")]
        public async Task<ActionResult<SingleEntityResponse<BookResponse>>> GetBookByKey([FromRoute] Guid bookKey)
        {
            var book = await mediator.Send(new GetBookByKeyQuery(bookKey));
            var responce = mapper.Map<BookResponse>(book);
            return Ok(new SingleEntityResponse<BookResponse>(responce));
        }

        [HttpPost]
        public async Task<ActionResult> CreateBook([FromBody] BookRequest request)
        {
            var command = mapper.Map<CreateBookCommand>(request);
            var book = await mediator.Send(command);
            var response = mapper.Map<BookResponse>(book);

            await hubContext.Clients.All.SendAsync("ReceiveBookUpdate", $"A new book with key {response.BookKey} was added.");

            return CreatedAtAction(nameof(GetBookByKey), new { bookKey = response.BookKey }, response);
        }

        [HttpPost("bulkadd")]
        public async Task<ActionResult> AddBooks([FromForm] BooksRequest request)
        {
            var command = mapper.Map<AddBooksCommand>(request);
            var books = await mediator.Send(command);
            var response = mapper.Map<IEnumerable<BookResponse>>(books);

            await hubContext.Clients.All.SendAsync("ReceiveBookUpdate", $"New {response.Count()} books were added.");

            return CreatedAtAction(nameof(GetPage), books);
        }

        [HttpPut("{bookKey}")]
        public async Task<ActionResult<SingleEntityResponse<BookResponse>>> PutBook(
            [FromRoute] Guid bookKey,
            [FromBody] BookRequest request)
        {
            var command = mapper.Map<UpdateBookCommand>(request);
            command.BookKey = bookKey;

            var data = await mediator.Send(command);
            var result = mapper.Map<BookResponse>(data);

            await hubContext.Clients.All.SendAsync("ReceiveBookUpdate", $"A book with key {result.BookKey} was updated.");

            return Ok(new SingleEntityResponse<BookResponse>(result));
        }

        [HttpDelete("{bookKey}")]
        public async Task<IActionResult> DeleteBook([FromRoute] Guid bookKey)
        {
            var command = new DeleteBookCommand(bookKey);

            await mediator.Send(command);

            await hubContext.Clients.All.SendAsync("ReceiveBookUpdate", $"A book with key {bookKey} was deleted.");

            return NoContent();
        }
    }
}
