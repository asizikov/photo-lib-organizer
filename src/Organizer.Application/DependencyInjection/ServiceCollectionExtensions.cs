using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Organizer.Application.Configuration;
using Organizer.Application.Services;
using Organizer.Domain.DependencyInjection;
using Organizer.Infrastructure.Persistence.DependencyInjection;

namespace Organizer.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services,
        IConfiguration contextConfiguration)
    {
        services.AddDomain();
        services.AddPersistence(contextConfiguration.GetConnectionString("PhotolibOrganizerDb")!);

        services.AddTransient<IFileDataExtractorService, FileDataExtractorService>();
        services.AddSingleton<IFileNameParser, FileNameParser>();
        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(typeof(Program).Assembly); });
        services.Configure<OrganizerOptions>(contextConfiguration.GetSection(OrganizerOptions.Key));
        return services;
    }
}