using Microsoft.EntityFrameworkCore;
using Sombra.Extensions;
using Sombra.Utils;

namespace Sombra.UnitTests;

public class PostExtensionTest
{
    [Fact]
    public async Task SortExtension_WhenSortingByPropAsc_ReturnsCorrectList()
    {
        var (context, postService) = PostTestHelper.SetupContext();
        
        var postsResult = await PostTestHelper.SetupPosts(context);
        
        var expected = postsResult.OrderBy(p => p.Title).ToList();

        var result = await context.Posts.ApplySort(true)
            .ToListAsync(cancellationToken: TestContext.Current.CancellationToken);
        
        Assert.True(expected.SequenceEqual(result));
    }
    
    [Fact]
    public async Task SortExtension_WhenSortingByPropDesc_ReturnsCorrectList()
    {
        var (context, postService) = PostTestHelper.SetupContext();
        
        var postsResult = await PostTestHelper.SetupPosts(context);
        
        var expected = postsResult.OrderByDescending(p => p.Title).ToList();

        var result = await context.Posts.ApplySort( false)
            .ToListAsync(cancellationToken: TestContext.Current.CancellationToken);
        
        Assert.True(expected.SequenceEqual(result));
    }
    
    [Fact]
    public async Task SortExtension_WhenSortingByCategory_ReturnsCorrectList()
    {
        var (context, postService) = PostTestHelper.SetupContext();
        
        var postsResult = await PostTestHelper.SetupPosts(context);
        
        var expected = postsResult.OrderBy(p => p.Category).ToList();

        var result = await context.Posts.ApplySort(true, "category")
            .ToListAsync(cancellationToken: TestContext.Current.CancellationToken);
        
        Assert.True(expected.SequenceEqual(result));
    }

    [Fact]
    public async Task SortExtension_WhenSortingDuplicateValues_ReturnsCorrectList()
    {
        var (context, postService) = PostTestHelper.SetupContext();
        
        await PostTestHelper.SetupPosts(context);

        List<string> tags = ["Test"];

        var post = new Post
        {
            Title = "Duplicated Post",
            Content = "This is a test duplicate post",
            Category = "Test",
            Tags = PostHelper.StringsToTags(tags)
        };
        
        await context.Posts.AddRangeAsync(Enumerable.Repeat(post, 2), TestContext.Current.CancellationToken);
        
        var expected = context.Posts.OrderBy(p => p.Title).ToList();
        
        var result = await context.Posts.ApplySort(true)
            .ToListAsync(cancellationToken: TestContext.Current.CancellationToken);
        
        Assert.True(expected.SequenceEqual(result));
    }

    [Fact]
    public async Task SortExtension_WhenSortingEmptyList_ReturnsEmptyList()
    {
        var (context, postService) = PostTestHelper.SetupContext();
        
        var result = await context.Posts.ApplySort(true)
            .ToListAsync(cancellationToken: TestContext.Current.CancellationToken);
        
        Assert.Empty(result);
    }
}