using Microsoft.AspNetCore.Mvc;

namespace Sombra.Models.DTOs;

public class QueryParams
{
    public string? Term {get; set;}
    
    public string? SortBy { get; set; }
    public bool? Ascending { get; set; } = true;
    
    [FromQuery(Name = "page")]
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}