using Microsoft.EntityFrameworkCore;
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

        var examplePost = PostUtils.ExamplePostDto();
        
        var tag = new Tag(examplePost.Tags[0]);
        context.Set<Tag>().Add(tag);

        List<Post> posts = [];
        
        for (int i = 0; i < 2; i++)
        {
            posts.Add(new Post
            {
                Title = examplePost.Title,
                Content = examplePost.Content,
                Category =  examplePost.Category,
                Tags = [tag]
            });
        }
        
        context.Set<Post>().AddRange(posts);
        
        
        context.SaveChanges();
    }
}