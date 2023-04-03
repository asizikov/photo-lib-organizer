namespace Organizer.Application.Services;

public interface IFileNameParser
{
    DateTime? ExtractDateFromFileName(string fileName);
}