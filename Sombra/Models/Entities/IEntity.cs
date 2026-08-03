namespace Sombra.Models.Entities;

public interface IEntity
{
    int Id  { get; }
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset? UpdatedAt { get; set; }
}