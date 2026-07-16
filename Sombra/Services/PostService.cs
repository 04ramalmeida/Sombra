using Microsoft.EntityFrameworkCore;
using Sombra.Endpoints;
using Sombra.Models.DTOs;
using Sombra.Models.Entities;

namespace Sombra.Services;

public class PostService(SombraDb db)
{
    private readonly SombraDb _db = db;

    internal async Task<List<PostResponseDto>> GetPostsAsync(string? term)
    {
        if (string.IsNullOrEmpty(term))
        { //TODO: can probably find a way to not repeat this
            return await db.Posts.Select(p => new PostResponseDto(
                    p.Id,
                    p.Title,
                    p.Content,
                    p.Category,
                    new List<string>(p.Tags.Select(t => t.Name))
                ))
                .ToListAsync();
        }
        
        return await db.Posts
                                        .Where(p => p.Title.ToLower().Contains(term) ||
                                         p.Content.ToLower().Contains(term) ||
                                         p.Category.ToLower().Contains(term) ||
                                         p.Tags.Any(t => EF.Functions.Like(t.Name, $"%{term}%"))
        ).Select(p => new PostResponseDto(p.Id,
                p.Title,
                p.Content,
                p.Category,
                p.Tags.Select(t => t.Name).ToList()))
            .ToListAsync();
        
    }

    internal async Task<PostResponseDto?> GetPostDtoAsync(int id)
    {
        return await db.Posts.Where(p => p.Id == id)
            .Select(p => new PostResponseDto(
                p.Id,
                p.Title,
                p.Content,
                p.Category,
                p.Tags.Select(t => t.Name).ToList())).FirstOrDefaultAsync();
    }

    internal async Task<Post?> GetPostByIdAsync(int id)
    {
        return await db.Posts.FindAsync(id);
    } 

    internal async Task<Post> CreatePostAsync(Post post)
    {
        await db.Posts.AddAsync(post);
        await db.SaveChangesAsync();
        return post;
    }

    internal async Task<Post> UpdatePostAsync(Post post,
        CreatePostDto input)
    {
        post.Title = input.Title;
        post.Content = input.Content;
        post.Category = input.Category;
        post.Tags = GetOrCreateTags(input.Tags);

        await db.SaveChangesAsync();
        
        return post;
    }
    
    internal async Task RemovePostAsync(Post post)
    {
        db.Posts.Remove(post);
        await db.SaveChangesAsync();
    }
    
    public List<Tag> GetOrCreateTags(List<string> tags)
    {
        List<Tag> result = [];
        
        result.AddRange(_db.Tags.Where(t => tags.Contains(t.Name)));
        
        List<Tag> newTags = 
            tags.Except(result.Select(t => t.Name))
                .Select(name => new Tag(name))
                .ToList();
        
        _db.Tags.AddRange(newTags);
        
        result.AddRange(newTags);
        
        return result;
    }
}