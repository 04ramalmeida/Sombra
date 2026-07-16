using Microsoft.EntityFrameworkCore;
using Sombra.Models.DTOs;
using Sombra.Models.Entities;

namespace Sombra.Utils;

public class PostUtils
{
    

    public static bool PostsContainsTerm(string term, List<PostResponseDto> posts, SombraDb context)
    {
        bool hasTerm = false;

        foreach (var post in posts)
        {
            hasTerm = hasTerm ||post.Title.Contains(term, StringComparison.OrdinalIgnoreCase);
            hasTerm = hasTerm || post.Content.Contains(term, StringComparison.OrdinalIgnoreCase);
            hasTerm = hasTerm || post.Category.Contains(term, StringComparison.OrdinalIgnoreCase);
            hasTerm = hasTerm || context.Tags
                .Any(t =>EF.Functions.Like(t.Name, $"%{term}%")) ;
        }
        
        return hasTerm;
    }
    
    public static Post ExamplePost() => new()
    {
        Title = "My First Blog Post",
        Content = "This is the content of my first blog post.",
        Category = "Programming",
        Tags = [new Tag("Tech")]
    };

    public static CreatePostDto ExamplePostDto() => new CreatePostDto(
        "My First Blog Post",
        "This is the content of my first blog post.",
        "Programming",
        ["Tech"]
    );

    public static PostResponseDto ToDto(Post post) => new PostResponseDto(
        post.Id,
        post.Title,
        post.Content,
        post.Category,
        post.Tags.Select(tag => tag.Name).ToList());
}