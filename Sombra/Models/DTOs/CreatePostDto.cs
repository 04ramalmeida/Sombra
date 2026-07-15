using System.ComponentModel.DataAnnotations;

namespace Sombra.Models.DTOs;

public record CreatePostDto(
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

public record PostResponseDto(
    int Id,
    string Title,
    string Content,
    string Category,
    List<string> Tags
);