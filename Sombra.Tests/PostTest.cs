using Sombra.Models.DTOs;
using Sombra.Services;


namespace Sombra.Tests;

public class PostTest
{
    private Post CreateExamplePost() => new()
    {
        Title = "My First Blog Post",
        Content = "This is the content of my first blog post.",
        Category = "Technology",
        Tags = new List<string> { "Tech", "Programming" }
    };

    private static (SombraDb, PostService) SetupContext()
    {
        var context = new MockDb().CreateDbContext();
        
        return (context, new PostService(context));
    }

    private static PostService SetupService()
    {
        var context = new MockDb().CreateDbContext();
        
        return (new PostService(context));
    }
    
    [Fact]
    public async Task GetPost_WhenPostExists_ReturnsCorrectPost()
    {
        var (context, postService) = SetupContext();
        
        var post = await SetupExamplePost(context);
        
        Assert.IsType<Post>(post);
        
        var result = await postService.GetPostAsync(post.Id);
        
        Assert.IsType<Post>(result);
        
        Assert.Equivalent(post, result);
    }

    [Fact]
    public async Task GetPost_WhenMissing_ReturnsNull()
    {
        var postService = SetupService();
        
        var result = await postService.GetPostAsync(1);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPosts_WhenTermMatches_ReturnsOk()
    {
        var (context, postService) = SetupContext();
        
        const string term = "tech";
        
        var post= await SetupExamplePost(context);

        Assert.IsType<Post>(post);
        
        var result = await postService.GetPostsAsync(term);
        
        Assert.IsType<List<Post>>(result);
        Assert.True(PostsContainsTerm(term, result));
    }
    
    [Fact]
    public async Task GetPosts_WhenTermDoesntMatch_ReturnsEmptyList()
    {
        var (context, postService) = SetupContext();
        
        const string term = "Painting";
        
        var postResult = await SetupExamplePost(context);
        
        Assert.IsType<Post>(postResult);
        
        var result = await postService.GetPostsAsync(term);
        Assert.IsType<List<Post>>(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task CreatePost_ReturnsCreatedPost()
    {
        var (context, postService) = SetupContext();
        
        var post = await postService.CreatePostAsync(CreateExamplePost());
        
        Assert.NotNull(post);
        var dbPost = context.Posts.FirstOrDefault(p => p.Id == post.Id);
        
        Assert.Equivalent(post, dbPost);
    }

    [Fact]
    public async Task UpdatePost_ReturnsUpdatedPost()
    {
        var (context, postService) = SetupContext();

        var post = await SetupExamplePost(context);
        
        var postId = post.Id;

        var update = new PostDto(
            "My Updated Blog Post",
            "This is the updated content of my first blog post.",
            "Technology",
            ["Tech", "Programming"]
        );

        var result = await postService.UpdatePostAsync(post, update);

        Assert.IsType<Post>(result);
        Assert.Equivalent(update, result);
        
        var dbResult = context.Posts.FirstOrDefault(p => p.Id == postId);
        
        Assert.NotNull(dbResult);
        Assert.Equivalent(update.Title, dbResult.Title);
        Assert.Equivalent(update.Content, dbResult.Content);
        Assert.Equivalent(update.Category, dbResult.Category);
        Assert.Equivalent(update.Tags, dbResult.Tags);
    }

    [Fact]
    public async Task DeletePost_DeletesThePost()
    {
        // Given
        var (context, postService) = SetupContext();

        var post = await SetupExamplePost(context);

        var postId = post.Id;
        // When
        await postService.RemovePostAsync(post);
        var dbResult = await context.Posts.FindAsync(postId);
        
        Assert.Null(dbResult);
    }
    
    private bool PostsContainsTerm(string term, List<Post> posts)
    {
        bool hasTerm = false;

        foreach (var post in posts)
        {
            hasTerm = post.Title.Contains(term, StringComparison.OrdinalIgnoreCase);
            hasTerm = hasTerm || post.Content.Contains(term, StringComparison.OrdinalIgnoreCase);
            hasTerm = hasTerm || post.Category.Contains(term, StringComparison.OrdinalIgnoreCase);
            bool tagFound = false;
            for (var index = 0; index < post.Tags.Count; )
            {
                var tag = post.Tags[index];
                while (!tagFound && index < post.Tags.Count)
                {
                    tagFound = tag.Contains(term, StringComparison.OrdinalIgnoreCase);
                }
                if (tagFound) hasTerm = hasTerm && tagFound;
                index++;  
            }
        }
        
        return hasTerm;
    }

    private async Task<Post> SetupExamplePost(SombraDb context)
    {
        var post = CreateExamplePost();
        await context.Posts.AddAsync(post);
        await context.SaveChangesAsync();
        return post;
    }
}
