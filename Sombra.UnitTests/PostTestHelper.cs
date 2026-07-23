using Sombra.Services;
using Sombra.Utils;

namespace Sombra.UnitTests;

public static class PostTestHelper
{
    public static (SombraDb, PostService) SetupContext()
    {
        var context = new MockDb().CreateDbContext();
        
        return (context, new PostService(context));
    }

    public static PostService SetupService()
    {
        var context = new MockDb().CreateDbContext();
        
        return (new PostService(context));
    }
    
    public static async Task<Post> SetupPost(SombraDb context)
    {
        var post = PostHelper.CreateToModel(PostHelper.GeneratePost());
        await context.Posts.AddAsync(post);
        await context.SaveChangesAsync();
        return post;
    }

    public static async Task<List<Post>> SetupPosts(SombraDb context)
    {
        var posts = PostHelper.CreateDtosToModel(PostHelper.GeneratePosts(10));
        await context.Posts.AddRangeAsync(posts);
        await context.SaveChangesAsync();
        return posts;
    }
}