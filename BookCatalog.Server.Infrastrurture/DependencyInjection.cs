using BookCatalog.Server.Infrastructure.Data;
using BookCatalog.Server.Infrastructure.Repositories;
using BookCatalog.Shared.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookCatalog.Server.Infrastrurture
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Add SQLite database

            services.AddDbContextFactory<ApplicationDbContext>(o => o.UseInMemoryDatabase(databaseName: "Books_Catalog"));

            services.AddScoped<IBookRepository, BookRepository>();

            return services;
        }
    }
}
