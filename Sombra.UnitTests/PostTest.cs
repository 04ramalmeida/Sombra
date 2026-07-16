using Sombra.Models.DTOs;
using Sombra.Services;
using Sombra.Utils;


namespace Sombra.UnitTests;

public class PostTest
{

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
        
        var result = await postService.GetPostDtoAsync(post.Id);
        
        Assert.IsType<PostResponseDto>(result);
        
        Assert.Equivalent(PostUtils.ExamplePostDto(), result);
    }

    [Fact]
    public async Task GetPost_WhenMissing_ReturnsNull()
    {
        var postService = SetupService();
        
        var result = await postService.GetPostDtoAsync(1);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPosts_WhenTermMatches_ReturnsOk()
    {
        var (context, postService) = SetupContext();
        
        const string term = "tech";
        
        var post= await SetupExamplePost(context);

        Assert.IsType<Post>(post);
        
        var result = await postService.GetPostsAsync(new QueryParams
        {
            SearchTerm = term
        });
        
        Assert.IsType<List<PostResponseDto>>(result);
        Assert.True(PostUtils.PostsContainsTerm(term, result, context));
    }
    
    [Fact]
    public async Task GetPosts_WhenTermDoesntMatch_ReturnsEmptyList()
    {
        var (context, postService) = SetupContext();
        
        const string term = "Painting";
        
        var postResult = await SetupExamplePost(context);
        
        Assert.IsType<Post>(postResult);
        
        var result = await postService.GetPostsAsync(new QueryParams
        {
            SearchTerm = term
        });
        Assert.IsType<List<PostResponseDto>>(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task CreatePost_ReturnsCreatedPost()
    {
        var (context, postService) = SetupContext();
        
        var post = await postService.CreatePostAsync(PostUtils.ExamplePost());
        
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

        var update = new CreatePostDto(
            "My Updated Blog Post",
            "This is the updated content of my first blog post.",
            "Technology",
            ["Tech", "Programming"]
        );

        var result = await postService.UpdatePostAsync(post, update);

        Assert.IsType<Post>(result);
        
        var dbResult = context.Posts.FirstOrDefault(p => p.Id == postId);
        
        Assert.NotNull(dbResult);
        Assert.Equivalent(update.Title, dbResult.Title);
        Assert.Equivalent(update.Content, dbResult.Content);
        Assert.Equivalent(update.Category, dbResult.Category);
        Assert.Equivalent(update.Tags, dbResult.Tags.Select(t=>t.Name));
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

    private async Task<Post> SetupExamplePost(SombraDb context)
    {
        var post = PostUtils.ExamplePost();
        await context.Posts.AddAsync(post);
        await context.SaveChangesAsync();
        return post;
    }
}
