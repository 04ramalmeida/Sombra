using Microsoft.EntityFrameworkCore;

public class SombraDb : DbContext
{
    public SombraDb(DbContextOptions<SombraDb> options)
        : base(options) { }

    public DbSet<Post> Posts => Set<Post>();
}