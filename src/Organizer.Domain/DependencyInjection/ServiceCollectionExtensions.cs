using Microsoft.Extensions.DependencyInjection;

namespace Organizer.Domain.DependencyInjection;

public static class ServiceCollectionExtensions
{
    // Add Domain services to the DI container
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        return services;
    }
}