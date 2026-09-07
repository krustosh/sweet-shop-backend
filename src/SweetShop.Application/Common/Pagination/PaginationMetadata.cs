namespace SweetShop.Application.Common.Pagination;

/// <summary>
/// Represents pagination metadata for a collection response.
/// </summary>
public sealed record PaginationMetadata(
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages)
{
    /// <summary>
    /// Gets a value indicating whether another page is available.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Gets a value indicating whether a previous page is available.
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}