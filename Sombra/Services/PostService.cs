using Microsoft.EntityFrameworkCore;
using Sombra.Endpoints;
using Sombra.Models.DTOs;

namespace Sombra.Services;

public class PostService(SombraDb db)
{
    private readonly SombraDb _db = db;

    internal async Task<List<Post>> GetPostsAsync(string? term)
    {
        if (string.IsNullOrEmpty(term))
        {
            return await db.Posts.ToListAsync();
        }
        
        return await db.Posts.Where(p => p.Title.ToLower().Contains(term) ||
                                         p.Content.ToLower().Contains(term) ||
                                         p.Category.ToLower().Contains(term)
        ).ToListAsync();
    }

    internal async Task<Post?> GetPostAsync(int id)
    {
        return await db.Posts.FindAsync(id);
    }

    internal async Task<Post> CreatePostAsync(Post post)
    {
        await db.Posts.AddAsync(post);
        await db.SaveChangesAsync();
        return post;
    }

    internal async Task<Post> UpdatePostAsync(Post post, PostDto input)
    {
        post.Title = input.Title;
        post.Content = input.Content;
        post.Category = input.Category;
        post.Tags = input.Tags.ToList();

        await db.SaveChangesAsync();
        
        return post;
    }
    
    internal async Task RemovePostAsync(Post post)
    {
        db.Posts.Remove(post);
        await db.SaveChangesAsync();
    }
}