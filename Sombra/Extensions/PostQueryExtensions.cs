using Microsoft.EntityFrameworkCore;

namespace Sombra.Extensions;

public static class PostQueryExtensions
{
    public static IQueryable<Post> ApplySearch(this IQueryable<Post> query, string? searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm)) return query;
        
        return query.Where(p => EF.Functions.Like(p.Title, $"%{searchTerm}%")
            || EF.Functions.Like(p.Content, $"%{searchTerm}%")
            || EF.Functions.Like(p.Category, $"%{searchTerm}%")
            || EF.Functions.Like(p.Category, $"%{searchTerm}%")
            || p.Tags.Any(t=> EF.Functions.Like(t.Name, $"%{searchTerm}%")));
    }

    public static IQueryable<Post> ApplySort(this IQueryable<Post> query, bool ascending, string sortBy = "title")
    {
        var sortProp = sortBy.ToLower();
        
        return sortProp switch
        {
            "id" => ascending ? query.OrderBy(p => p.Id) : query.OrderByDescending(p => p.Id),
            "title" => ascending ? query.OrderBy(p => p.Title) : query.OrderByDescending(p => p.Title),
            "category" => ascending ? query.OrderBy(p => p.Category) : query.OrderByDescending(p => p.Category),
            _ => throw new ArgumentException($"Unknown sort property: {sortProp}")
        };
    }

    // TODO: Migrate to seek pagination after implementing indexes
    public static IQueryable<Post> ApplyPagination(this IQueryable<Post> query, int page = 1, int pageSize = 5)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 5;
        
        int firstId = (page - 1) * pageSize;

        return query 
            .OrderBy(p => p.Id)
            .Skip(firstId)
            .Take(pageSize);
    }
}