using Microsoft.EntityFrameworkCore;
using Organizer.Domain.Entities;

namespace Organizer.Infrastructure.Persistence;

public interface IApplicationDbContext
{
    DbSet<PhotoFile> PhotoFiles { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}