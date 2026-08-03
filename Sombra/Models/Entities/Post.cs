using System.Text.Json.Serialization;

namespace Sombra.Models.Entities;

public class Post : IEntity
{
    public int Id { get; init; }

    public required string Title { get; set; }

    public required string Content { get; set; }

    public required string Category { get; set; }

    [JsonIgnore]
    public List<Tag> Tags { get; set; } = [];
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }
}