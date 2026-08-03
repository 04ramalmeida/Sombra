using Bogus;
using Sombra.Models.DTOs;
using Sombra.Models.Entities;

namespace Sombra.Helpers;

public static class PostHelper
{
    private static readonly Faker Faker = new();

    public static bool PostsContainsTerm(string term, List<PostResponseDto> posts, SombraDb context)
    {
        bool hasTerm = true;

        foreach (var post in posts)
        {
            hasTerm = hasTerm && PostContainsTerm(term, post, context);
            if (!hasTerm) break;
        }
        
        return hasTerm;
    }

    private static bool PostContainsTerm(string term, PostResponseDto post, SombraDb context)
    {
        var hasTerm = false;
        hasTerm = hasTerm || post.Title.Contains(term, StringComparison.OrdinalIgnoreCase);
        hasTerm = hasTerm || post.Content.Contains(term, StringComparison.OrdinalIgnoreCase);
        hasTerm = hasTerm || post.Category.Contains(term, StringComparison.OrdinalIgnoreCase);
        hasTerm = hasTerm || post.Tags.Contains(term, StringComparer.OrdinalIgnoreCase) ;
        return hasTerm;
    }

    public static PostResponseDto ToResponseDto(Post post) => new PostResponseDto(
        post.Id,
        post.Title,
        post.Content,
        post.Category,
        post.Tags.Select(tag => tag.Name).ToList(),
        post.CreatedAt,
        post.UpdatedAt);
    
    public static List<PostResponseDto> ToResponseDtos(List<Post> posts) => posts.Select(ToResponseDto).ToList();
    
    public static CreatePostDto ToCreateDto(Post post) => new CreatePostDto(
        post.Title,
        post.Content,
        post.Category,
        post.Tags.Select(tag => tag.Name).ToList());

    public static Post CreateToModel(CreatePostDto dto) => new Post
    {
        Title = dto.Title,
        Content = dto.Content,
        Category = dto.Category,
        Tags = StringsToTags(dto.Tags)
    }; 

    public static CreatePostDto GeneratePost() =>
        new(
            Faker.Lorem.Sentence(3),
            Faker.Lorem.Lines(5),
            Faker.Commerce.ProductAdjective(),
            GenerateTags(Random.Shared.Next(1, 10))
        );

    public static List<CreatePostDto> GeneratePosts(int count)
    {
        var result = new List<CreatePostDto>();
        for (int i = 0; i < count; i++)
        {
            result.Add(GeneratePost());
        }
        return result;
    }

    public static List<Post> CreateDtosToModel(List<CreatePostDto> dtos)
    {
        var result = new List<Post>();
        foreach (var dto in dtos)
        {
            result.Add(new Post
            {
                Title = dto.Title,
                Content = dto.Content,
                Category = dto.Category,
                Tags = StringsToTags(dto.Tags)
            });
        }
        return result;
    }
    
    private static List<string> GenerateTags(int count)
    {
        var result = new List<string>();

        for (int i = 0; i < count; i++)
        {
            result.Add(Faker.Random.Word());
        }
        
        return result;
    }
    
    // This is going to be useful for when we verify 
    // if we can search through tags
    
    public static List<Tag> GetOrCreateTags(List<string> tags, SombraDb context)
    {
        List<Tag> result = [];


        result.AddRange(context.Tags.Where(t => tags.Contains(t.Name)).ToList()
            .Concat(context.Tags.Local.Where(t => tags.Contains(t.Name))));
            
        
        List<Tag> newTags = 
            tags.Except(result.Select(t => t.Name), StringComparer.CurrentCultureIgnoreCase)
                .Select(name => new Tag(name))
                .ToList();
        
        context.Tags.AddRange(newTags);
        
        result.AddRange(newTags);
        
        return result;
    }
    
    
    public static List<Tag> StringsToTags(List<string> strings)
    {
        List<Tag> result = [];
        foreach (var name in strings)
        {
            result.Add(new Tag(name));
        }
        return result;
    }
    
}

public class PostResponseDtoComparer : EqualityComparer<PostResponseDto>
{
    public override bool Equals(PostResponseDto? x, PostResponseDto? y)
    {
        if (x is null && y is null)
        {
            return true;
        } 
        if (x is null || y is null) return false;
        
        return x.Id ==  y.Id
               && x.Title == y.Title
               &&  x.Category == y.Category
               && x.Content == y.Content
               && x.Tags.SequenceEqual(y.Tags) 
               && x.CreatedAt == y.CreatedAt
               && x.UpdatedAt == y.UpdatedAt;
    }

    public override int GetHashCode(PostResponseDto? obj)
    {

        int tagsHash = 0;
        
        if (obj == null)
        {
            return HashCode.Combine(0);
        }
        
        foreach (var tag in obj.Tags)
        {
            tagsHash = HashCode.Combine(tagsHash, tag.GetHashCode());
        }

        return HashCode.Combine(obj.Id, obj.Title, obj.Category, obj.Content, obj.CreatedAt, obj.UpdatedAt,  tagsHash);
        
    }
}