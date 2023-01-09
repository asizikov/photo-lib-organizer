using System.Threading.Channels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organizer.Infrastructure.Persistence;

namespace Organizer.Application.Commands;

public class UpdateFileCreatedDatesCommandHandler : IRequestHandler<UpdateFileCreatedDatesCommand>
{
    private readonly IMediator mediator;
    private readonly ILogger<UpdateFileCreatedDatesCommandHandler> logger;
    private readonly IApplicationDbContext context;

    public UpdateFileCreatedDatesCommandHandler(IMediator mediator, ILogger<UpdateFileCreatedDatesCommandHandler> logger, IApplicationDbContext context)
    {
        this.mediator = mediator;
        this.logger = logger;
        this.context = context;
    }
    
    public async Task<Unit> Handle(UpdateFileCreatedDatesCommand request, CancellationToken cancellationToken)
    {
        var channel = Channel.CreateBounded<FileData>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = true,
            SingleReader = false
        });
        var writer = channel.Writer;
        var consumerTasks = InitConsumerTasks(channel, cancellationToken);

        var producerTask = Task.Run(() => ProducerTask(writer, cancellationToken), cancellationToken);

        await Task.WhenAll(producerTask, Task.WhenAll(consumerTasks));
        
        return Unit.Value;
    }
    
    private async Task ProducerTask(ChannelWriter<FileData> writer, CancellationToken stoppingToken)
    {
        logger.LogInformation("ProducerTasks started");

        await foreach (var file in  context.PhotoFiles
                           .AsNoTracking()
                           .Where(x => x.PhotoTaken != x.FileCreated && x.PhotoTaken != null)
                           .AsAsyncEnumerable().WithCancellation(stoppingToken))
        {
            if (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("CancellationRequested");
                break;
            }

            await writer.WriteAsync(new FileData(file.FilePath, file.PhotoTaken!.Value), stoppingToken);
        }

        writer.Complete();
    }

    private IEnumerable<Task> InitConsumerTasks(Channel<FileData> channel, CancellationToken stoppingToken)
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
            logger.LogInformation("Consumer Thread {TaskId} started", taskId);
            var counter = 0L;
            while (await channel.Reader.WaitToReadAsync(stoppingToken))
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogInformation("CancellationRequested stopping thread {TaskId}", taskId);
                    break;
                }

                if (channel.Reader.TryRead(out var fileData))
                {
                   File.SetCreationTime(fileData.FilePath,fileData.PhotoTaken);
                   counter++;
                }
            }
            logger.LogInformation("Update File Created Dates Thread {TaskId} finished. Updated {Counter} files", taskId, counter);
        }
    }

    private record FileData(string FilePath, DateTime PhotoTaken);
}