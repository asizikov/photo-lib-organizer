using Microsoft.EntityFrameworkCore;
using Organizer.Domain.Entities;

namespace Organizer.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<PhotoFile> PhotoFiles { get; set; }
}