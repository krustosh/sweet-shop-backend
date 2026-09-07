namespace SweetShop.Application.Common.Pagination;

/// <summary>
/// Represents a paginated application result.
/// </summary>
/// <typeparam name="T">The type of collection item.</typeparam>
public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    PaginationMetadata Pagination);