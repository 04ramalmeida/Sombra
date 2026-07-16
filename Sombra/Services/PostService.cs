using Microsoft.EntityFrameworkCore;
using Sombra.Endpoints;
using Sombra.Extensions;
using Sombra.Models.DTOs;
using Sombra.Models.Entities;

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