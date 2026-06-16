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
            bool tagFound = false;
            for (var index = 0; index < post.Tags.Count; )
            {
                var tag = post.Tags[index];
                while (!tagFound && index < post.Tags.Count)
                {
                    tagFound = tag.Contains(term, StringComparison.OrdinalIgnoreCase);
                }
                if (tagFound) hasTerm = hasTerm && tagFound;
                index++;  
            }
        }
        
        return hasTerm;
    }
}