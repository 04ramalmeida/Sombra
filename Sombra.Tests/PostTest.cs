using Microsoft.AspNetCore.Http.HttpResults;
using Xunit.Internal;

namespace Sombra.Tests;

public class PostTest
{

    [Fact]
    public async Task GettingPostReturnsOkWithPost()
    {
        await using var context = new MockDb().CreateDbContext();
        
        var post = new PostsEndpoints.PostDto(
            "My First Blog Post",
            "This is the content of my first blog post.",
            "Technology",
            ["Tech", "Programming"]
        );

        var postResult = await PostsEndpoints.CreatePost(post, context);


        Assert.IsType<Created<Post>>(postResult);

        var createdResult = (Created<Post>)postResult;
        var postId = createdResult.Value.Id;
        
        var result = await PostsEndpoints.GetPost(postId, context);
        
        Assert.IsType<Ok<Post>>(result);
        
        var getResult = (Ok<Post>)result;
        
        Assert.NotNull(getResult.Value);
        Assert.Equivalent(post, getResult.Value);
    }

    [Fact]
    public async Task GettingNonExistentPostReturnsNotFound()
    {
        await using var context = new MockDb().CreateDbContext();
        
        var result = await PostsEndpoints.GetPost(1, context);
        
        Assert.IsType<NotFound>(result);
    }

    [Fact]
    public async Task GettingPostsWithSearchTermReturnsOkWithRelevantPosts()
    {
        var term = "tech";
        
        await using var context = new MockDb().CreateDbContext();
        
        var post = new PostsEndpoints.PostDto(
            "My First Blog Post",
            "This is the content of my first blog post.",
            "Technology",
            ["Tech", "Programming"]
        );

        var postResult = await PostsEndpoints.CreatePost(post, context);

        Assert.IsType<Created<Post>>(postResult);
        
        var result = await PostsEndpoints.GetPosts(term, context);
        
        var okResult = Assert.IsType<Ok<List<Post>>>(result);
        Assert.NotEmpty(okResult.Value);
        Assert.True(PostsContainsTerm(term, okResult.Value));
    }
    
    [Fact]
    public async Task NonMatchingTermReturnsEmpty()
    {
        var term = "Painting";
        
        await using var context = new MockDb().CreateDbContext();
        
        var post = new PostsEndpoints.PostDto(
            "My First Blog Post",
            "This is the content of my first blog post.",
            "Technology",
            ["Tech", "Programming"]
        );
        
        var postResult = await PostsEndpoints.CreatePost(post, context);
        
        Assert.IsType<Created<Post>>(postResult);
        
        var result = await PostsEndpoints.GetPosts(term, context);
        var getResult = Assert.IsType<Ok<List<Post>>>(result);
        
        Assert.Empty(getResult.Value);
    }
    
    [Fact]
    public async Task PostingPostReturnsThePost()
    {
        await using var context = new MockDb().CreateDbContext();

        var post = new PostsEndpoints.PostDto(
            "My First Blog Post",
            "This is the content of my first blog post.",
            "Technology",
            ["Tech", "Programming"]
        );

        var result = await PostsEndpoints.CreatePost(post, context);

        Assert.IsType<Created<Post>>(result);

        var createdResult = (Created<Post>)result;

        Assert.NotNull(createdResult.Value);
        Assert.Equivalent(post, createdResult.Value);
    }

    [Fact]
    public async Task UpdatingTaskReturnsTheUpdatedTask()
    {
        await using var context = new MockDb().CreateDbContext();

        var post = new PostsEndpoints.PostDto(
            "My First Blog Post",
            "This is the content of my first blog post.",
            "Technology",
            ["Tech", "Programming"]
        );

        var postResult = await PostsEndpoints.CreatePost(post, context);


        Assert.IsType<Created<Post>>(postResult);

        var createdResult = (Created<Post>)postResult;
        var postId = createdResult.Value.Id;

        var update = new PostsEndpoints.PostDto(
            "My Updated Blog Post",
            "This is the updated content of my first blog post.",
            "Technology",
            ["Tech", "Programming"]
        );

        var result = await PostsEndpoints.UpdatePost(postId, update, context);

        Assert.IsType<Ok<Post>>(result);

        var updatedResult = (Ok<Post>)result;

        Assert.NotNull(updatedResult.Value);
        Assert.Equivalent(update, updatedResult.Value);
        
        var dbResult = context.Posts.FirstOrDefault(p => p.Id == postId);
        
        Assert.NotNull(dbResult);
        Assert.Equivalent(update.Title, dbResult.Title);
        Assert.Equivalent(update.Content, dbResult.Content);
        Assert.Equivalent(update.Category, dbResult.Category);
        Assert.Equivalent(update.Tags, dbResult.Tags);
    }

    [Fact]
    public async Task UpdatingNonExistantTaskReturnsNotFound()
    {
        await using var context = new MockDb().CreateDbContext();

        var update = new PostsEndpoints.PostDto(
            "My Updated Blog Post",
            "This is the updated content of my first blog post.",
            "Technology",
            ["Tech", "Programming"]
        );

        var result = await PostsEndpoints.UpdatePost(1, update, context);

        Assert.IsType<NotFound>(result);
    }

    [Fact]
    public async Task SuccessfulDeleteReturnsNoContent()
    {
        // Given
        await using var context = new MockDb().CreateDbContext();

        var post = new PostsEndpoints.PostDto(
            "My First Blog Post",
            "This is the content of my first blog post.",
            "Technology",
            ["Tech", "Programming"]);
        
        var postResult = await PostsEndpoints.CreatePost(post, context);


        Assert.IsType<Created<Post>>(postResult);

        var createdResult = (Created<Post>)postResult;
        var postId = createdResult.Value.Id;
        // When
        var result = await PostsEndpoints.DeletePost(postId, context);
        var dbResult = await context.Posts.FindAsync(postId);

        // Then
        Assert.IsType<NoContent>(result);
        Assert.Null(dbResult);
    }

    [Fact]
    public async Task NonExistentPostDeleteReturnsNotFound()
    {
        await using var context = new MockDb().CreateDbContext();
        
        var result = await PostsEndpoints.DeletePost(1, context);
        var searchResult = context.Posts.FirstOrDefault(p => p.Id == 1);
        
        Assert.IsType<NotFound>(result);
        Assert.Null(searchResult);
        
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
                    if (tagFound) hasTerm = tagFound;
                    index++;
                }
            }
        }
        
        return hasTerm;
    }
}
