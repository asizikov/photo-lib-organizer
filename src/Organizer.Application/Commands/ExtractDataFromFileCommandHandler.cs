using MediatR;
using Microsoft.Extensions.Logging;
using Organizer.Application.Services;
using Organizer.Infrastructure.Persistence;

namespace Organizer.Application.Commands;

public class ExtractDataFromFileCommandHandler : IRequestHandler<ExtractDataFromFileCommand, Guid>
{
    private readonly IFileDataExtractorService fileDataExtractorService;
    private readonly IApplicationDbContext dbContext;
    private readonly ILogger<ExtractDataFromFileCommandHandler> logger;

    public ExtractDataFromFileCommandHandler(IFileDataExtractorService fileDataExtractorService,
        IApplicationDbContext dbContext, ILogger<ExtractDataFromFileCommandHandler> logger)
    {
        this.fileDataExtractorService = fileDataExtractorService;
        this.dbContext = dbContext;
        this.logger = logger;
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
            logger.LogError(ex,"Error while saving file to database");
        }

        return Guid.Empty;
    }
}