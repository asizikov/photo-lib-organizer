using System.Globalization;
using System.Security.Cryptography;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Microsoft.Extensions.Logging;
using Organizer.Domain.Entities;

namespace Organizer.Application.Services;

public class FileDataExtractorService : IFileDataExtractorService
{
    private readonly ILogger<FileDataExtractorService> logger;
    private readonly HashSet<string> knownPhotoExtensions = new();

    public FileDataExtractorService(ILogger<FileDataExtractorService> logger)
    {
        this.logger = logger;
        knownPhotoExtensions.Add(".jpg");
        knownPhotoExtensions.Add(".jpeg");
        knownPhotoExtensions.Add(".heic");
    }

    public async Task<PhotoFile> ExtractFileDataAsync(string filePath, CancellationToken cancellationToken)
    {
        var fileInfo = new FileInfo(filePath);

        var photoFileEntity = new PhotoFile
        {
            FileName = Path.GetFileName(filePath),
            FileExtension = Path.GetExtension(filePath),
            FilePath = filePath,
            FileSize = fileInfo.Length,
            FileCreated = fileInfo.CreationTime,
        };

        if (knownPhotoExtensions.Contains(fileInfo.Extension.ToLower()))
        {
            // extract date from exif data
            try
            {
                await using var fileStream = fileInfo.OpenRead();
                var metadata = ImageMetadataReader.ReadMetadata(fileStream);

                var exifSubIfdDirectory = metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                if (exifSubIfdDirectory is not null)
                {
                    var dateDescription = exifSubIfdDirectory.GetDescription(ExifDirectoryBase.TagDateTimeOriginal);

                    if (DateTime.TryParseExact(dateDescription, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out var date))
                    {
                        photoFileEntity.PhotoTaken = date;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "failed to extract metadata for file {FilePath}", filePath);
            }
        }

        photoFileEntity.Hash = await CalculateFileHashAsync(fileInfo, cancellationToken);

        return photoFileEntity;
    }

    private async Task<string?> CalculateFileHashAsync(FileInfo fileInfo, CancellationToken cancellationToken)
    {
        await using var fileStream = fileInfo.OpenRead();
        using var md5 = MD5.Create();
        var hash = await md5.ComputeHashAsync(fileStream, cancellationToken);
        return Convert.ToBase64String(hash);
    }
}