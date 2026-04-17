public class Post
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public required string Content { get; set; }

    public required string Category { get; set; }

    public required List<string> Tags { get; set; }
}