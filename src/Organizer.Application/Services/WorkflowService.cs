using Organizer.Domain.Entities;
using Organizer.Infrastructure.Persistence;

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
        foreach (var filePath in Directory.EnumerateFiles(@"D:\yadisk_photos_full_dump\YandexDisk\Фотокамера_copy"))
        {
            Console.WriteLine(filePath);
            context.PhotoFiles.Add(new PhotoFile
            {
                Id = Guid.NewGuid(),
                FileName = Path.GetFileName(filePath),
                FileExtension = Path.GetExtension(filePath),
                FilePath = filePath
            });
            await context.SaveChangesAsync(CancellationToken.None);
        }
    }
}