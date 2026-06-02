using System.ComponentModel.DataAnnotations;

namespace Sombra.Endpoints;

public record PostDto(
    [property: Required]
    [property: StringLength(64, MinimumLength = 8)]
    string Title,
    [property: Required]
    string Content,
    [property: Required]
    [property: StringLength(24)]
    string Category,
    [property: MaxLength(10)]
    List<string> Tags
);