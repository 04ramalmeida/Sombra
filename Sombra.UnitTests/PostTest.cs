using Microsoft.EntityFrameworkCore;
using Sombra.Extensions;
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
        
        var post = await SetupPost(context);
        
        Assert.IsType<Post>(post);
        
        var result = await postService.GetPostDtoAsync(post.Id);
        
        Assert.IsType<PostResponseDto>(result);
        
        Assert.Equivalent(PostUtils.ToCreateDto(post), result);
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
        
        var posts = await SetupPosts(context);
        
        var tags = posts.SelectMany(p => p.Tags)
            .Select(t => t.Name)
            .ToList();

        var term = tags.GetRndTag();
        
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
        
        const string term = "zzzzzzzzzzzzzzzzzzzzzz((((((((((("; 
        
        var postsResult = await SetupPosts(context);
        
        Assert.IsType<List<Post>>(postsResult);
        
        var result = await postService.GetPostsAsync(new QueryParams
        {
            SearchTerm = term
        });
        Assert.IsType<List<PostResponseDto>>(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SortExtension_WhenSortingByPropAsc_ReturnsCorrectList()
    {
        var (context, postService) = SetupContext();
        
        var postsResult = await SetupPosts(context);
        
        var expected = postsResult.OrderBy(p => p.Title).ToList();

        //var result = await context.Posts.ApplySort("title").ToListAsync;
        
        Assert.IsType<List<PostResponseDto>>(result);
        Assert.Equivalent(expected, result);
    }
    
    [Fact]
    public async Task SortExtension_WhenSortingByPropDesc_ReturnsCorrectList()
    {
        var (context, postService) = SetupContext();
        
        var postsResult = await SetupPosts(context);
        
        var expected = postsResult.OrderByDescending(p => p.Title).ToList();

        //var result = await context.Posts.ApplySort("title", false).ToListAsync;
        
        Assert.IsType<List<PostResponseDto>>(result);
        Assert.Equivalent(expected, result);
    }
    
    [Fact]
    public async Task SortExtension_WhenSortingByCategory_ReturnsCorrectList()
    {
        var (context, postService) = SetupContext();
        
        var postsResult = await SetupPosts(context);
        
        var expected = postsResult.OrderBy(p => p.Category).ToList();

        //var result = await context.Posts.ApplySort("category").ToListAsync;
        
        Assert.IsType<List<PostResponseDto>>(result);
        Assert.Equivalent(expected, result);
    }

    [Fact]
    public async Task SortExtension_WhenSortingDuplicateValues_ReturnsCorrectList()
    {
        var (context, postService) = SetupContext();
        
        await SetupPosts(context);

        List<string> tags = ["Test"];

        var post = new Post
        {
            Title = "Duplicated Post",
            Content = "This is a test duplicate post",
            Category = "Test",
            Tags = PostUtils.StringsToTags(tags)
        };
        
        await context.Posts.AddRangeAsync(Enumerable.Repeat(post, 2), TestContext.Current.CancellationToken);
        
        var expected = context.Posts.OrderBy(p => p.Title).ToList();
        
        //var result = await context.Posts.ApplySort("title").ToListAsync;
        
        Assert.IsType<List<PostResponseDto>>(result);
        Assert.Equivalent(expected, result);
    }

    [Fact]
    public async Task SortExtension_WhenSortingEmptyList_ReturnsEmptyList()
    {
        var (context, postService) = SetupContext();
        
        //var result = await context.Posts.ApplySort("title").ToListAsync;
        
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task CreatePost_ReturnsCreatedPost()
    {
        var (context, postService) = SetupContext();
        
        var post = await postService.CreatePostAsync(PostUtils.GeneratePost());
        
        Assert.NotNull(post);
        var dbPost = context.Posts.FirstOrDefault(p => p.Id == post.Id);
        
        Assert.Equivalent(post, dbPost);
    }

    [Fact]
    public async Task UpdatePost_ReturnsUpdatedPost()
    {
        var (context, postService) = SetupContext();

        var post = await SetupPost(context);
        
        var postId = post.Id;

        var update = new CreatePostDto(
            "My Updated Blog Post",
            "This is the updated content of my first blog post.",
            "Technology",
            ["Tech", "Programming"]
        );

        var result = await postService.UpdatePostAsync(post, update);

        Assert.IsType<Post>(result);
        
        var dbResult = context.Posts.Where(p => p.Id == postId)
            .Select(p => new PostResponseDto(
                p.Id,
                p.Title,
                p.Content,
                p.Category,
                p.Tags.Select(t => t.Name).ToList()
                )).FirstOrDefault();
        
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

        var post = await SetupPost(context);

        var postId = post.Id;
        // When
        await postService.RemovePostAsync(post);
        var dbResult = await context.Posts.FindAsync(postId);
        
        Assert.Null(dbResult);
    }

    private static async Task<Post> SetupPost(SombraDb context)
    {
        var post = PostUtils.CreateToModel(PostUtils.GeneratePost());
        await context.Posts.AddAsync(post);
        await context.SaveChangesAsync();
        return post;
    }

    private async Task<List<Post>> SetupPosts(SombraDb context)
    {
        var posts = PostUtils.CreateDtosToModel(PostUtils.GeneratePosts(10));
        await context.Posts.AddRangeAsync(posts);
        await context.SaveChangesAsync();
        return posts;
    }
}
