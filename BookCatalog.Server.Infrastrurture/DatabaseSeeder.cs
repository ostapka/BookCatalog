using BookCatalog.Server.Domain.Entities;
using BookCatalog.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookCatalog.Server.Infrastrurture
{
    public static class DatabaseSeeder
    {
        public static void SeedDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            using var dbContext = factory.CreateDbContext();

            if (dbContext.Database.IsInMemory())
            {
                dbContext.Database.EnsureCreated();

                if (!dbContext.Books.Any())
                {
                    dbContext.Books.AddRange(new[]
                    {
                        new Book
                        {
                            BookKey = Guid.NewGuid(),
                            Title = "Sample Book 1",
                            Author = "Author 1",
                            Genre = "Fiction",
                            PublishedDate = DateTime.Now.AddYears(-1)
                        },
                        new Book
                        {
                            BookKey = Guid.NewGuid(),
                            Title = "Sample Book 2",
                            Author = "Author 2",
                            Genre = "Science",
                            PublishedDate = DateTime.Now.AddYears(-2)
                        }
                    });

                    dbContext.SaveChanges();
                }
            }
        }
    }
}
