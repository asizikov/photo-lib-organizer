using MediatR;

namespace Organizer.Application.Commands;

public class ExtractDataFromFileCommand: IRequest<Guid>
{
    public string FilePath { get; set; } = string.Empty;
}