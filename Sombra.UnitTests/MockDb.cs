using Microsoft.EntityFrameworkCore;
using Sombra;

public class MockDb : IDbContextFactory<SombraDb>
{
    public SombraDb CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SombraDb>()
            .UseInMemoryDatabase($"TestDb-{DateTime.Now.ToFileTimeUtc()}")
            .Options;

        return new SombraDb(options);
    }
}