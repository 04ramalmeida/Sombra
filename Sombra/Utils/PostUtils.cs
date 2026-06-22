namespace Sombra.Utils;

public class PostUtils
{
    public static bool PostsContainsTerm(string term, List<Post> posts)
    {
        bool hasTerm = false;

        foreach (var post in posts)
        {
            hasTerm = post.Title.Contains(term, StringComparison.OrdinalIgnoreCase);
            hasTerm = hasTerm || post.Content.Contains(term, StringComparison.OrdinalIgnoreCase);
            hasTerm = hasTerm || post.Category.Contains(term, StringComparison.OrdinalIgnoreCase);
            // TODO: Uncomment when tags have been reworked
            // for (var index = 0; index < post.Tags.Count; index++)
            // {
            //     var tag = post.Tags[index];
            //     hasTerm = hasTerm || tag.Contains(term, StringComparison.OrdinalIgnoreCase);
            // }
        }
        
        return hasTerm;
    }
    
    public static Post ExamplePost() => new()
    {
        Title = "My First Blog Post",
        Content = "This is the content of my first blog post.",
        Category = "Technology",
        Tags = ["Tech", "Programming"]
    };
}