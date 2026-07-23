using Microsoft.EntityFrameworkCore;
using Sombra.Endpoints;
using Sombra.Extensions;
using Sombra.Models.DTOs;
using Sombra.Models.Entities;
using Sombra.Utils;

namespace Sombra.Services;

public class PostService(SombraDb db)
{
    private readonly SombraDb _db = db;
    
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

    internal async Task<List<PostResponseDto>> GetPostsAsync(QueryParams parameters)
    {
        var query = db.Posts.AsNoTracking().AsQueryable();
        
        //Apply search filtering to the query
        query = query.ApplySearch(parameters.SearchTerm);
        
        return await query.Select(p => new PostResponseDto(
            p.Id,
            p.Title,
            p.Content,
            p.Category,
            p.Tags.Select(t => t.Name).ToList()))
            .ToListAsync();
    }

    internal async Task<Post?> GetPostByIdAsync(int id)
    {
        return await db.Posts.FindAsync(id);
    } 

    internal async Task<Post> CreatePostAsync(CreatePostDto input)
    {
        var post = new Post
        {
            Title = input.Title,
            Content = input.Content,
            Category = input.Category,
            Tags = PostHelper.GetOrCreateTags(input.Tags, _db)
        };
        
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
        post.Tags = PostHelper.GetOrCreateTags(input.Tags, _db);

        await db.SaveChangesAsync();
        
        return post;
    }
    
    internal async Task RemovePostAsync(Post post)
    {
        db.Posts.Remove(post);
        await db.SaveChangesAsync();
    }
    
    
}