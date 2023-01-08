using System.Reflection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Organizer.Application.DependencyInjection;
using Organizer.Application.Services;

ThreadPool.SetMinThreads(100, 100);

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((builder =>
    {
        builder.Sources.Clear();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json", false, false);
    }))
    .ConfigureServices((context, services) =>
    {
        services.AddApplication(context.Configuration);
        services.AddHostedService<WorkflowService>();
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting Photo Organizer");

await host.RunAsync();

logger.LogInformation("Finished Photo Organizer");