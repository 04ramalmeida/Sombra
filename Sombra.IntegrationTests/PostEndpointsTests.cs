using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Sombra.Models.DTOs;
using Sombra.Services;
using Sombra.Utils;

namespace Sombra.IntegrationTests;

public class PostEndpointsTests: IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly SombraDb _context;

    public PostEndpointsTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        var scope = _factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<SombraDb>();
        _client = factory.CreateClient();
    }


    [Fact]
    public async Task GetPosts_ReturnsOkAndPosts()
    {
        var response = await _client.GetAsync("/api/posts");
        
        var posts = await response.Content.ReadFromJsonAsync<List<Post>>();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(posts);
    }

    [Fact]
    public async Task GetPosts_WhenTermIncluded_ReturnsOkAndPosts()
    {
        var response = await _client.GetAsync("/api/posts?term=tech");
        
        var posts = await response.Content.ReadFromJsonAsync<List<PostResponseDto>>();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(posts);
        Assert.NotEmpty(posts);
        Assert.True(PostUtils.PostsContainsTerm("tech", posts, _context));
    }

    [Fact]
    public async Task CreatePost_WhenInputValid_ReturnsCreatedPost()
    {
        var input = new CreatePostDto(
            "Created Test Post",
            "This is the content of a post created by an integration test.",
            "Test",
            ["Test"]
        );
        
        var response = await _client.PostAsJsonAsync("/api/posts", input);
        
        var post = await response.Content.ReadFromJsonAsync<PostResponseDto>();
        
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
        var input = new CreatePostDto(
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
        var input = new CreatePostDto(
            "Updated Test Post",
            "This is the content of a post updated by an integration test.",
            "Test",
            ["NewTag"]);
        
        Post? dbPost = _context.Posts.FirstOrDefault();
        
        Assert.NotNull(dbPost);
        
        var response = await _client.PutAsJsonAsync($"/api/posts/{dbPost.Id}", input);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var post = await response.Content.ReadFromJsonAsync<PostResponseDto>();
        
        Assert.NotNull(post);
        Assert.Equal(input.Title, post.Title);
        Assert.Equal(input.Content, post.Content);
        Assert.Equal(input.Category, post.Category);
        Assert.Equal(input.Tags, post.Tags);
    }

    [Fact]
    public async Task UpdatePost_WhenInputInvalid_ReturnsBadRequest()
    {
        var input = new CreatePostDto(
            "BadTitle",
            "This is the content of a post update by an integration test.",
            "This is a very long and extensive category name, and it won't pass validation",
            ["Test"]);
        
        
        
        var response = await _client.PutAsJsonAsync("/api/posts/2", input);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePost_WhenPostNotExist_ReturnsNotFound()
    {
        var input = new CreatePostDto(
            "Updated Test Post",
            "This is the content of a post updated by an integration test.",
            "Test",
            ["Test"]);
        
        var response = await _client.PutAsJsonAsync("/api/posts/50868795", input);
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeletePost_WhenPostExists_ReturnsNoContent()
    {
        Post? post = _context.Posts.FirstOrDefault();
        
        Assert.NotNull(post);
        
        var response = await _client.DeleteAsync($"/api/posts/{post.Id}");
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeletePost_WhenPostDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/posts/50868795");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}