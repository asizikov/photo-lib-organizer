using MediatR;
using Organizer.Domain.Entities;

namespace Organizer.Application.Commands;

public class CreateOrUpdateFileCommand : IRequest<Guid>
{
    public PhotoFile File { get; set; }
}