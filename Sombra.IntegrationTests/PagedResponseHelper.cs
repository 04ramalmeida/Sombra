using Sombra.Models.DTOs;

namespace Sombra.IntegrationTests;

public static class PagedResponseHelper
{
    public static void AssertResponsePropsEquality(
        (int pageNumber, int pageSize, int totalPages, int totalRecords, bool hasNextPage, bool hasPreviousPage) expected,
        PagedResponse<PostResponseDto> response)
    {
        Assert.Equal(expected.pageNumber, response.PageNumber);
        Assert.Equal(expected.pageSize, response.PageSize);
        Assert.Equal(expected.totalPages, response.TotalPages);
        Assert.Equal(expected.totalRecords, response.TotalRecords);
        Assert.Equal(expected.hasNextPage, response.HasNextPage);
        Assert.Equal(expected.hasPreviousPage, response.HasPreviousPage);
    } 
}