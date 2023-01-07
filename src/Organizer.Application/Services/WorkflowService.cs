using System.Diagnostics;
using Organizer.Domain.Entities;
using Organizer.Infrastructure.Persistence;
using System.Threading.Channels;

namespace Organizer.Application.Services;

public class WorkflowService : IWorkflowService
{
    private readonly IApplicationDbContext context;
    public WorkflowService(IApplicationDbContext context)
    {
        this.context = context;
    }
    
    public async Task RunAsync()
    {
        Console.WriteLine("WorkflowService.RunAsync");
        await StartReadingFilesAsync();
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
        InitFileDataExtractors(channel);
        var directory = @"/Users/asizikov/Downloads";
        // var directory = @"D:\yadisk_photos_full_dump\YandexDisk\Фотокамера_copy";
        foreach (var filePath in Directory.EnumerateFiles(directory))
        {
            await writer.WriteAsync(filePath);
            // Console.WriteLine(filePath);
            // context.PhotoFiles.Add(new PhotoFile
            // {
            //     Id = Guid.NewGuid(),
            //     FileName = Path.GetFileName(filePath),
            //     FileExtension = Path.GetExtension(filePath),
            //     FilePath = filePath
            // });
            // await context.SaveChangesAsync(CancellationToken.None);
        }
        writer.Complete();
    }

    private async Task InitFileDataExtractors(Channel<string> channel)
    {
        for (var i = 0; i < 10; i++)
        {
            _ = Task.Factory.StartNew( async () =>
            {
                var id = i;
                while (await channel.Reader.WaitToReadAsync()) 
                {
                    if (channel.Reader.TryRead(out var filePath)) 
                    {
                        Console.WriteLine(id + "--" + filePath);
                    }
                }
            });
        }
    }
}
