
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("Sombra.Tests")]
public static class PostsEndpoints
{

    public record PostDto(
        [property: Required]
        [property: StringLength(64, MinimumLength = 8)]
        string Title,
        [property: Required]
        string Content,
        [property: Required]
        [property: StringLength(24)]
        string Category,
        [property: MaxLength(10)]
        List<string> Tags
    );

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

    internal static async Task<IResult> GetPosts(string? searchTerm, SombraDb db)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            var posts = await db.Posts.ToListAsync();
            return TypedResults.Ok(posts);
        }

        // To find a better solution for case insensitivity
        var results = await db.Posts.Where(p => p.Title.ToLower().Contains(searchTerm) ||
                            p.Content.ToLower().Contains(searchTerm) ||
                            p.Category.ToLower().Contains(searchTerm)
                            ).ToListAsync();

        return TypedResults.Ok(results);
    }

    internal static async Task<IResult> GetPost(int id, SombraDb db)
    {
        var post = await db.Posts.FindAsync(id);
        if (post is null) return TypedResults.NotFound();

        return TypedResults.Ok(post);
    }

    internal static async Task<IResult> CreatePost(PostDto input, SombraDb db)
    {
        var post = new Post
        {
            Title = input.Title,
            Content = input.Content,
            Category = input.Category,
            Tags = input.Tags.ToList()
        };

        db.Posts.Add(post);
        await db.SaveChangesAsync();

        return TypedResults.Created($"/posts/{post.Id}", post);
    }

    internal static async Task<IResult> UpdatePost(int id, PostDto input, SombraDb db)
    {
        var post = await db.Posts.FindAsync(id);
        if (post is null) return TypedResults.NotFound();

        post.Title = input.Title;
        post.Content = input.Content;
        post.Category = input.Category;
        post.Tags = input.Tags.ToList();

        await db.SaveChangesAsync();
        return TypedResults.Ok(post);
    }

    internal static async Task<IResult> DeletePost(int id, SombraDb db)
    {
        var post = await db.Posts.FindAsync(id);
        if (post is null) return TypedResults.NotFound();

        db.Posts.Remove(post);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}