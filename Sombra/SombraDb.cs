using Microsoft.EntityFrameworkCore;
using Sombra.Models.Entities;

namespace Sombra;

public class SombraDb : DbContext
{
    public SombraDb(DbContextOptions<SombraDb> options)
        : base(options) { }

    public DbSet<Post> Posts => Set<Post>();
    
    public DbSet<Tag> Tags => Set<Tag>();
}