using Microsoft.AspNetCore.Http.HttpResults;

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

}
