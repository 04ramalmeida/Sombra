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
}