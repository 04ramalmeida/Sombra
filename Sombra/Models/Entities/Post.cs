using System.Text.Json.Serialization;
using Sombra.Models.Entities;

public class Post
{
    public int Id { get; init; }

    public required string Title { get; set; }

    public required string Content { get; set; }

    public required string Category { get; set; }

    [JsonIgnore]
    public List<Tag> Tags { get; set; } = [];
}