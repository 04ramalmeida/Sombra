using System.Runtime.CompilerServices;
using Sombra.Models.DTOs;
using Sombra.Services;
using Sombra.Utils;

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

    private static async Task<IResult> GetPosts([AsParameters]QueryParams parameters, PostService postService)
    {
        return TypedResults.Ok(await postService.GetPostsAsync(parameters));
    }

    private static async Task<IResult> GetPost(int id, PostService postService)
    {
        var post = await postService.GetPostDtoAsync(id);
        if (post is null) return TypedResults.NotFound();

        return TypedResults.Ok(post);
    }

    private static async Task<IResult> CreatePost(CreatePostDto input,
        PostService postService)
    {

        var result = await postService.CreatePostAsync(input);

        var dto = PostUtils.ToDto(result);
        
        return TypedResults.Created($"/posts/{result.Id}", dto);
    }

    private static async Task<IResult> UpdatePost(int id, CreatePostDto input, PostService postService)
    {
        var post = await postService.GetPostByIdAsync(id);
        if (post is null) return TypedResults.NotFound();

        var result = await postService.UpdatePostAsync(post, input);
        
        var dto = PostUtils.ToDto(result);
        
        return TypedResults.Ok(dto);
    }

    private static async Task<IResult> DeletePost(int id, PostService postService)
    {
        var post = await postService.GetPostByIdAsync(id);
        if (post is null) return TypedResults.NotFound();

        await postService.RemovePostAsync(post);
        return TypedResults.NoContent();
    }
}