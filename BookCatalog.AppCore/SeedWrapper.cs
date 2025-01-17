using BookCatalog.Server.Infrastrurture;

namespace BookCatalog.Server.AppCore
{
    public static class SeedWrapper
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            DatabaseSeeder.SeedDatabase(serviceProvider);
        }
    }
}
