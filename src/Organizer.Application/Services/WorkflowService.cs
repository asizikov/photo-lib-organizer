using System.Diagnostics;
using Organizer.Domain.Entities;
using Organizer.Infrastructure.Persistence;
using System.Threading.Channels;
using MediatR;
using Organizer.Application.Commands;

namespace Organizer.Application.Services;

public class WorkflowService : IWorkflowService
{
    private readonly IMediator mediator;

    public WorkflowService(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("WorkflowService.RunAsync");
        var stopwatch = Stopwatch.StartNew();
        await StartReadingFilesAsync();
        Console.WriteLine($"WorkflowService.RunAsync: {stopwatch.ElapsedMilliseconds} ms");
    }

    private async Task StartReadingFilesAsync()
    {
        var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = true,
            SingleReader = false
        });
        var writer = channel.Writer;
        var extractorTasks = InitFileDataExtractors(channel);
        var directory = @"D:\yadisk_photos_full_dump\YandexDisk\Фотокамера_copy";
        foreach (var filePath in Directory.EnumerateFiles(directory))
        {
            await writer.WriteAsync(filePath);
        }

        writer.Complete();

        await Task.WhenAll(extractorTasks);
    }

    private List<Task> InitFileDataExtractors(Channel<string> channel)
    {
        var tasks = new List<Task>();
        for (var i = 0; i < 10; i++)
        {
            tasks.Add(
                Task.Factory.StartNew(async () =>
                {
                    var taskId = Guid.NewGuid();
                    Console.WriteLine($"Data Extractor Thread {taskId} started");
                    var counter = 0L;
                    while (await channel.Reader.WaitToReadAsync())
                    {
                        if (channel.Reader.TryRead(out var filePath))
                        {
                            await mediator.Send(new ExtractDataFromFileCommand { FilePath = filePath });
                            counter++;
                        }
                    }

                    Console.WriteLine($"Data Extractor Thread {taskId} finished, inserted {counter} records");
                }));
        }

        return tasks;
    }
}