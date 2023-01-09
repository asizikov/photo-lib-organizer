using MediatR;

namespace Organizer.Application.Commands;

public class BuildIndexCommand : IRequest
{
    public string SourceDirectory { get; set; } = string.Empty;
}