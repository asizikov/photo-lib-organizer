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
    private readonly IMediator mediator;

    public ExtractDataFromFileCommandHandler(IFileDataExtractorService fileDataExtractorService,
        IApplicationDbContext dbContext, ILogger<ExtractDataFromFileCommandHandler> logger, IMediator mediator)
    {
        this.fileDataExtractorService = fileDataExtractorService;
        this.dbContext = dbContext;
        this.logger = logger;
        this.mediator = mediator;
    }

    public async Task<Guid> Handle(ExtractDataFromFileCommand request, CancellationToken cancellationToken)
    {
        var file = await fileDataExtractorService.ExtractFileDataAsync(request.FilePath, cancellationToken);
        try
        {
            var id = await mediator.Send(new CreateOrUpdateFileCommand { File = file} , cancellationToken);
            return id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,"Error while saving file to database");
        }

        return Guid.Empty;
    }
}