using System.Diagnostics;
using System.Threading.Channels;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Organizer.Application.Commands;

namespace Organizer.Application.Services;

public class WorkflowService : BackgroundService
{
    private readonly IMediator mediator;
    private readonly ILogger<WorkflowService> logger;
    private readonly IHostApplicationLifetime hostApplicationLifetime;

    public WorkflowService(IMediator mediator, ILogger<WorkflowService> logger, IHostApplicationLifetime hostApplicationLifetime)
    {
        this.mediator = mediator;
        this.logger = logger;
        this.hostApplicationLifetime = hostApplicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("WorkflowService.RunAsync");
        var stopwatch = Stopwatch.StartNew();
        await StartReadingFilesAsync(stoppingToken);
        logger.LogInformation($"WorkflowService.RunAsync: {stopwatch.ElapsedMilliseconds} ms");
    }

    private async Task StartReadingFilesAsync(CancellationToken stoppingToken)
    {
        var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = true,
            SingleReader = false
        });
        var writer = channel.Writer;
        var extractorTasks = InitFileDataExtractors(channel, stoppingToken);
        var directory = @"D:\yadisk_photos_full_dump\YandexDisk\Фотокамера_copy";
        //var directory = @"D:\yadisk_photos\Photos and videos from Yandex.Disk";
        
        var producerTask = Task.Run(() => ProducerTask(writer, directory, stoppingToken), stoppingToken);

        await Task.WhenAll(producerTask, Task.WhenAll(extractorTasks));

        hostApplicationLifetime.StopApplication();
    }
    
    private async Task ProducerTask(ChannelWriter<string> writer, string directory, CancellationToken stoppingToken)
    {
        logger.LogInformation("ProducerTasks started");
        foreach (var filePath in Directory.EnumerateFiles(directory))
        {
            if (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("CancellationRequested");
                break;
            }
            await writer.WriteAsync(filePath, stoppingToken);
        }
        writer.Complete();
    }

    private IEnumerable<Task> InitFileDataExtractors(Channel<string> channel, CancellationToken stoppingToken)
    {
        var tasks = new List<Task>();
        for (var i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() => ConsumerTask(), CancellationToken.None));
        }

        return tasks;

        async Task ConsumerTask()
        {
            var taskId = Guid.NewGuid();
            var counter = 0L;
            logger.LogInformation("Data Extractor Thread {TaskId} started", taskId);
            while (await channel.Reader.WaitToReadAsync(stoppingToken))
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogInformation("CancellationRequested stopping thread {TaskId}", taskId);
                    break;
                }
                if (channel.Reader.TryRead(out var filePath))
                {
                    await mediator.Send(new ExtractDataFromFileCommand { FilePath = filePath }, stoppingToken);
                    counter++;
                }
            }
            logger.LogInformation("Data Extractor Thread {TaskId} finished, inserted {Counter} records", taskId, counter);
        }
    }
}