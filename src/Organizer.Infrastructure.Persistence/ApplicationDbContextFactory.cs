using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Organizer.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
       // optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=photo-organizer-db;Trusted_Connection=True;Trust Server Certificate=true;MultipleActiveResultSets=true");
        optionsBuilder.UseSqlServer(@"Server=.\\db,1443;Database=photo-organizer-db;User Id=sa;Password=P@ssw0rd;");
"
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}