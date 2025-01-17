using BookCatalog.Server.Infrastrurture;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BookCatalog.Server.AppCore
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppCore(this IServiceCollection services)
        {
            services.AddMediatR(cnf => cnf.RegisterServicesFromAssemblies(
                Assembly.GetExecutingAssembly()));
            services.AddInfrastructure();

            return services;
        }
    }
}
