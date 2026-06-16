using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Sombra.Models.DTOs;
using Sombra.Services;

[assembly: InternalsVisibleTo("Sombra.UnitTests")]
namespace Sombra.Endpoints;

public static class PostsEndpoints
{
    public static RouteGroupBuilder MapPostsApi(this RouteGroupBuilder group)
    {
        var posts = group.MapGroup("/posts");

        posts.MapGet("/", GetPosts);
        posts.MapGet("/{id}", GetPost);
        posts.MapPost("/", CreatePost);
        posts.MapPut("/{id}", UpdatePost);
        posts.MapDelete("/{id}", DeletePost);

        return posts;
    }

    private static async Task<IResult> GetPosts(string? term, PostService postService)
    {
        return TypedResults.Ok(await postService.GetPostsAsync(term));
    }

    private static async Task<IResult> GetPost(int id, PostService postService)
    {
        var post = await postService.GetPostAsync(id);
        if (post is null) return TypedResults.NotFound();

        return TypedResults.Ok(post);
    }

    private static async Task<IResult> CreatePost(PostDto input, PostService postService)
    {
        var post = new Post
        {
            Title = input.Title,
            Content = input.Content,
            Category = input.Category,
            Tags = input.Tags.ToList()
        };

        var result = await postService.CreatePostAsync(post);

        return TypedResults.Created($"/posts/{result.Id}", result);
    }

    private static async Task<IResult> UpdatePost(int id, PostDto input, PostService postService)
    {
        var post = await postService.GetPostAsync(id);
        if (post is null) return TypedResults.NotFound();

        var result = await postService.UpdatePostAsync(post, input);
        
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> DeletePost(int id, PostService postService)
    {
        var post = await postService.GetPostAsync(id);
        if (post is null) return TypedResults.NotFound();

        await postService.RemovePostAsync(post);
        return TypedResults.NoContent();
    }
}