using Microsoft.EntityFrameworkCore;
using Sombra.Endpoints;
using Sombra.Extensions;
using Sombra.Helpers;
using Sombra.Models.DTOs;
using Sombra.Models.Entities;

namespace Sombra.Services;

public class PostService(SombraDb db)
{
    private readonly SombraDb _db = db;
    
    internal async Task<PostResponseDto?> GetPostDtoAsync(int id)
    {
        return await db.Posts.Where(p => p.Id == id)
            .Select(p => new PostResponseDto( //TODO: maybe replace this with the helper method?
                p.Id,
                p.Title,
                p.Content,
                p.Category,
                p.Tags.Select(t => t.Name).ToList(),
                p.CreatedAt,
                p.UpdatedAt)).FirstOrDefaultAsync();
    }

    internal async Task<PagedResponse<PostResponseDto>> GetPostsAsync(QueryParams parameters)
    {
        var pageNumber = Math.Max(1, parameters.PageNumber ?? 1);
        var pageSize = Math.Clamp(parameters.PageSize ?? 5, 1, 15);
        
        var query = db.Posts.AsNoTracking().AsQueryable();
        
        //Apply search filtering to the query
        query = query.ApplySearch(parameters.Term);
        
        var totalRecords = await query.CountAsync();
        
        //Apply sorting to the query
        query = query.ApplySort(parameters.Ascending ?? true , parameters.SortBy ?? "title");
        
        //Apply pagination to the query
        query = query.ApplyPagination(parameters.PageNumber ?? 1, parameters.PageSize ?? 5);
        
        var data = await query.Select(p => new PostResponseDto( //TODO: maybe replace this with the helper method?
            p.Id,
            p.Title,
            p.Content,
            p.Category,
            p.Tags.Select(t => t.Name).ToList(),
            p.CreatedAt,
            p.UpdatedAt))
            .ToListAsync();

        return new PagedResponse<PostResponseDto>
        {
            Data = data,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
            TotalRecords = totalRecords
        };
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