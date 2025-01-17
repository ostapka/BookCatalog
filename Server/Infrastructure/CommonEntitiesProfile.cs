using AutoMapper;
using BookCatalog.Shared.Models;
using BookCatalog.Shared.Request.Sorting;

namespace BookCatalog.Server.Infrastructure
{
    public class CommonEntitiesProfile : Profile
    {
        public CommonEntitiesProfile()
        {
            CreateMap<ClientSort, ClientSortDto>();
        }
    }
}
