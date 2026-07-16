namespace Sombra.Models.DTOs;

public class QueryParams
{
    public string? SearchTerm {get; set;}
    
    public string? SortBy { get; set; }
    public bool? Ascending { get; set; }
    
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}