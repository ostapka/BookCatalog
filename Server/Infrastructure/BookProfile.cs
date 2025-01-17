using AutoMapper;
using BookCatalog.Server.AppCore.Books.Commands;
using BookCatalog.Server.Domain.Entities;
using BookCatalog.Shared.Contracts.Response;
using BookCatalog.Shared.Models;
using BookCatalog.Shared.Request.Books;

namespace BookCatalog.Server.Infrastructure
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<BookDto, BookResponse>();
            CreateMap<Book, BookDto>().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
            CreateMap<CreateBookCommand, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.BookKey, opt => opt.Ignore());
            CreateMap<BookRequest, CreateBookCommand>();
            CreateMap<UpdateBookCommand, Book>().ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<BookRequest, UpdateBookCommand>().ForMember(dest => dest.BookKey, opt => opt.Ignore());
            CreateMap<BooksRequest, AddBooksCommand>();
        }
    }
}
