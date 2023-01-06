using Microsoft.Extensions.DependencyInjection;
using Organizer.Application.Services;
using Organizer.Domain.DependencyInjection;
using Organizer.Infrastructure.Persistence.DependencyInjection;

namespace Organizer.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddDomain();
        services.AddPersistence();
        //services.AddInfrastructure();
        services.AddTransient<IWorkflowService, WorkflowService>();
        return services;
    }
}