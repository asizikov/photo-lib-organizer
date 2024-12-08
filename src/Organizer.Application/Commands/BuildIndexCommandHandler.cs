using System.Threading.Channels;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Organizer.Application.Commands;

public class BuildIndexCommandHandler : IRequestHandler<BuildIndexCommand>
{
    private readonly IMediator mediator;
    private readonly ILogger<BuildIndexCommandHandler> logger;

    public BuildIndexCommandHandler(IMediator mediator,ILogger<BuildIndexCommandHandler> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    public async Task Handle(BuildIndexCommand request, CancellationToken cancellationToken)
    {
        await StartReadingFilesAsync(request.SourceDirectory, cancellationToken);
    }

    private async Task StartReadingFilesAsync(string sourceDirectory, CancellationToken stoppingToken)
    {
        var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = true,
            SingleReader = false
        });
        var writer = channel.Writer;
        var extractorTasks = InitFileDataExtractors(channel, stoppingToken);
        var directory = sourceDirectory;

        var producerTask = Task.Run(() => ProducerTask(writer, directory, stoppingToken), stoppingToken);

        await Task.WhenAll(producerTask, Task.WhenAll(extractorTasks));
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
            tasks.Add(Task.Run(ConsumerTask, CancellationToken.None));
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

            logger.LogInformation("Data Extractor Thread {TaskId} finished, inserted {Counter} records", taskId,
                counter);
        }
    }
}