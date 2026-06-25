using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Sombra.Models.DTOs;
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

    [Fact]
    public async Task CreatePost_WhenInputValid_ReturnsCreatedPost()
    {
        var input = new PostDto(
            "Created Test Post",
            "This is the content of a post created by an integration test.",
            "Test",
            ["Test"]
        );
        
        var response = await _client.PostAsJsonAsync("/api/posts", input);
        
        var post = await response.Content.ReadFromJsonAsync<Post>();
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(post);
        Assert.Equal(input.Title, post.Title);
        Assert.Equal(input.Content, post.Content);
        Assert.Equal(input.Category, post.Category);
        Assert.Equal(input.Tags, post.Tags);
    }

    [Fact]
    public async Task CreatePost_WhenInputInvalid_ReturnsBadRequest()
    {
        var input = new PostDto(
            "BadTitle",
            "This is the content of a post created by an integration test.",
            "This is a very long and extensive category name, and it won't pass validation",
            ["Test"]);
        
        var response = await _client.PostAsJsonAsync("/api/posts", input);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePost_WhenInputValid_ReturnsUpdatedPost()
    {
        var input = new PostDto(
            "Updated Test Post",
            "This is the content of a post updated by an integration test.",
            "Test",
            ["Test"]);
        
        var response = await _client.PutAsJsonAsync("/api/posts/1", input);
        
        var post = await response.Content.ReadFromJsonAsync<Post>();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(post);
        Assert.Equal(input.Title, post.Title);
        Assert.Equal(input.Content, post.Content);
        Assert.Equal(input.Category, post.Category);
        Assert.Equal(input.Tags, post.Tags);
    }

    [Fact]
    public async Task UpdatePost_WhenInputInvalid_ReturnsBadRequest()
    {
        var input = new PostDto(
            "BadTitle",
            "This is the content of a post update by an integration test.",
            "This is a very long and extensive category name, and it won't pass validation",
            ["Test"]);
        
        var response = await _client.PutAsJsonAsync("/api/posts/1", input);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePost_WhenPostNotExist_ReturnsNotFound()
    {
        var input = new PostDto(
            "Updated Test Post",
            "This is the content of a post updated by an integration test.",
            "Test",
            ["Test"]);
        
        var response = await _client.PutAsJsonAsync("/api/posts/50868795", input);
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}