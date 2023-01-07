using MediatR;
using Organizer.Application.Services;
using Organizer.Infrastructure.Persistence;

namespace Organizer.Application.Commands;

public class ExtractDataFromFileCommandHandler : IRequestHandler<ExtractDataFromFileCommand, Guid>
{
    private readonly IFileDataExtractorService fileDataExtractorService;
    private readonly IApplicationDbContext dbContext;

    public ExtractDataFromFileCommandHandler(IFileDataExtractorService fileDataExtractorService,
        IApplicationDbContext dbContext)
    {
        this.fileDataExtractorService = fileDataExtractorService;
        this.dbContext = dbContext;
    }

    public async Task<Guid> Handle(ExtractDataFromFileCommand request, CancellationToken cancellationToken)
    {
        var file = await fileDataExtractorService.ExtractFileDataAsync(request.FilePath, cancellationToken);
        try
        {
            dbContext.PhotoFiles.Add(file);
            await dbContext.SaveChangesAsync(cancellationToken);

            return file.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while saving file to database" + ex.Message);
        }

        return Guid.Empty;
    }
}