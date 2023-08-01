using Microsoft.EntityFrameworkCore;

namespace BaseReact.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<DbUser?> Users => Set<DbUser>();
}