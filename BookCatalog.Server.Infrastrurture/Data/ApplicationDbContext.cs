using BookCatalog.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.Server.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
    }
}
