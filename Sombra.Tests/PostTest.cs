using Microsoft.AspNetCore.Http.HttpResults;
using Xunit.Internal;

namespace Sombra.Tests;

public class PostTest
{
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

}
