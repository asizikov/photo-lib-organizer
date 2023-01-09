using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organizer.Domain.Entities;
using Organizer.Infrastructure.Persistence;

namespace Organizer.Application.Queries;

public class GetFilesWithMismatchingDatesQuery : IRequest, IRequest<IEnumerable<PhotoFile>>
{
}

public class GetFilesWithMismatchingDatesQueryHandler : IRequestHandler<GetFilesWithMismatchingDatesQuery, IEnumerable<PhotoFile>>
{
    private readonly IApplicationDbContext context;
    private readonly ILogger<GetFilesWithMismatchingDatesQueryHandler> logger;

    public GetFilesWithMismatchingDatesQueryHandler(IApplicationDbContext context, ILogger<GetFilesWithMismatchingDatesQueryHandler> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<IEnumerable<PhotoFile>> Handle(GetFilesWithMismatchingDatesQuery request, CancellationToken cancellationToken)
    {
        return null;
    }
}