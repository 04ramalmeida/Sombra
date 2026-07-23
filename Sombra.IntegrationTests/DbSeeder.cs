using Microsoft.EntityFrameworkCore;
using Sombra.Models.DTOs;
using Sombra.Models.Entities;
using Sombra.Utils;

namespace Sombra.IntegrationTests;

public class DbSeeder
{
    public static void InitDb(DbContext context)
    {
        if (context.Set<Post>().Any())
        {
            context.Set<Post>().RemoveRange(context.Set<Post>());
        }

        if (context.Set<Tag>().Any())
        {
            context.Set<Tag>().RemoveRange(context.Set<Tag>());
        }

        var generatedPosts = PostHelper.GeneratePosts(10);

        List<Post> posts = CreateDtosToModel(generatedPosts, (SombraDb)context);
        
        context.Set<Post>().AddRange(posts);

        context.SaveChanges();
    }
    
    public static List<Post> CreateDtosToModel(List<CreatePostDto> dtos, SombraDb context)
    {
        var result = new List<Post>();
        foreach (var dto in dtos)
        {
            result.Add(new Post
            {
                Title = dto.Title,
                Content = dto.Content,
                Category = dto.Category,
                Tags = PostHelper.GetOrCreateTags(dto.Tags, context)
            });
        }
        return result;
    }
}