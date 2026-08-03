using Microsoft.EntityFrameworkCore;
using Sombra.Models.Entities;

namespace Sombra;

public class SombraDb : DbContext
{
    public SombraDb(DbContextOptions<SombraDb> options)
        : base(options)
    {
    }
    
    public DbSet<Post> Posts => Set<Post>();
    
    public DbSet<Tag> Tags => Set<Tag>();

    public override int SaveChanges()
    {
        SetTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    private void SetTimestamps()
    {
        foreach (var entity in ChangeTracker.Entries()
                     .Where(e => e.State == EntityState.Added)
                     .Select(e => e.Entity)
                     .OfType<IEntity>())
        {
            entity.CreatedAt = DateTimeOffset.UtcNow;
        }
        
        foreach (var entity in ChangeTracker.Entries()
                     .Where(e => e.State == EntityState.Modified)
                     .Select(e => e.Entity)
                     .OfType<IEntity>())
        {
            entity.UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}