using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Organizer.Infrastructure.Persistence.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(@"Server=.\SQLEXPRESS;Database=photo-organizer-db;Trust Server Certificate=true;Trusted_Connection=True;MultipleActiveResultSets=true"));
        
        services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(ServiceLifetime.Transient);
        return services;
    }
}