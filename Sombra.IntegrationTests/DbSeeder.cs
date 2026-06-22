using Microsoft.EntityFrameworkCore;
using Sombra.Utils;

namespace Sombra.IntegrationTests;

public class DbSeeder
{
    public static void CreatePosts(DbContext context)
    {
        if (context.Set<Post>().Any()) return;
        context.Set<Post>().Add(PostUtils.ExamplePost());
        context.SaveChanges();
    }
}