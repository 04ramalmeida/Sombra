using Microsoft.EntityFrameworkCore;

class SombraDb : DbContext
{
    public SombraDb(DbContextOptions<SombraDb> options)
        : base(options) { }

    public DbSet<Post> Posts => Set<Post>();
}