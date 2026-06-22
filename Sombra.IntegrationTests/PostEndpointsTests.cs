using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Sombra.Services;
using Sombra.Utils;

namespace Sombra.IntegrationTests;

public class PostEndpointsTests: IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PostEndpointsTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }


    [Fact]
    public async Task GetPosts_ReturnsOkandPosts()
    {
        var response = await _client.GetAsync("/api/posts");
        
        var posts = await response.Content.ReadFromJsonAsync<List<Post>>();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(posts);
    }

    [Fact]
    public async Task GetPosts_WhenTermIncluded_ReturnsOkandPosts()
    {
        var response = await _client.GetAsync("/api/posts?term=tech");
        
        var posts = await response.Content.ReadFromJsonAsync<List<Post>>();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(posts);
        Assert.NotEmpty(posts);
        Assert.True(PostUtils.PostsContainsTerm("tech", posts));
    }
    
}