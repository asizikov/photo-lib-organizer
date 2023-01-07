using Organizer.Domain.Entities;

namespace Organizer.Application.Services;

public interface IFileDataExtractorService
{
    Task<PhotoFile> ExtractFileDataAsync(string filePath, CancellationToken cancellationToken);
}