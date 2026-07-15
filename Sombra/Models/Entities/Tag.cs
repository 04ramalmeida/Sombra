using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Sombra.Models.Entities;

[Index(nameof(Name), IsUnique = true)]
public class Tag(string name)
{
    public int Id { get; init; }
    public string Name { get; set; } = name;
    
    [JsonIgnore]
    public List<Post> Posts { get; set; } = [];
    
}