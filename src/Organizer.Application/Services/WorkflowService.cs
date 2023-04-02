using System.Diagnostics;
using System.Threading.Channels;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Organizer.Application.Commands;
using Organizer.Application.Configuration;

namespace Organizer.Application.Services;

public class WorkflowService : BackgroundService
{
    private readonly IMediator mediator;
    private readonly ILogger<WorkflowService> logger;
    private readonly IHostApplicationLifetime hostApplicationLifetime;
    private readonly IOptions<OrganizerOptions> config;

    public WorkflowService(IMediator mediator, ILogger<WorkflowService> logger,
        IHostApplicationLifetime hostApplicationLifetime, IOptions<OrganizerOptions> config)
    {
        this.mediator = mediator;
        this.logger = logger;
        this.hostApplicationLifetime = hostApplicationLifetime;
        this.config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("WorkflowService.RunAsync");

        var stopwatch = Stopwatch.StartNew();
        
        foreach (var sourceDirectory in config.Value.DirectoriesToProcess)
        {
            await mediator.Send(new BuildIndexCommand { SourceDirectory = sourceDirectory }, stoppingToken);
        }

        await mediator.Send(new UpdateFileCreatedDatesCommand(), stoppingToken);

        logger.LogInformation("WorkflowService.RunAsync: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
        hostApplicationLifetime.StopApplication();
    }
}