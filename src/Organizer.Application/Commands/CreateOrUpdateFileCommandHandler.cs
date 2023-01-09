using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organizer.Infrastructure.Persistence;

namespace Organizer.Application.Commands;

public class CreateOrUpdateFileCommandHandler : IRequestHandler<CreateOrUpdateFileCommand, Guid>
{
    private readonly IApplicationDbContext dbContext;
    private readonly ILogger<CreateOrUpdateFileCommandHandler> logger;

    public CreateOrUpdateFileCommandHandler(IApplicationDbContext dbContext,
        ILogger<CreateOrUpdateFileCommandHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Guid> Handle(CreateOrUpdateFileCommand request, CancellationToken cancellationToken)
    {
        var existingFile = await dbContext.PhotoFiles.FirstOrDefaultAsync(f => f.FilePath == request.File.FilePath,
            cancellationToken: cancellationToken);
        if (existingFile is null)
        {
            dbContext.PhotoFiles.Add(request.File);
        }
        else
        {
            existingFile.FileName = request.File.FileName;
            existingFile.FileExtension = request.File.FileExtension;
            existingFile.FileSize = request.File.FileSize;
            existingFile.FileDescription = request.File.FileDescription;
            existingFile.PhotoTaken = request.File.PhotoTaken;
            existingFile.FileCreated = request.File.FileCreated;
            existingFile.Hash = request.File.Hash;

            dbContext.PhotoFiles.Update(existingFile);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return existingFile?.Id ?? request.File.Id;
    }
}