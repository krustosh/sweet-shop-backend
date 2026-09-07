namespace SweetShop.Application.Common.Pagination;

/// <summary>
/// Represents pagination parameters for an application query.
/// </summary>
public sealed record PaginationRequest
{
    /// <summary>
    /// Gets the requested page number.
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Gets the requested number of items per page.
    /// </summary>
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Validates and returns the pagination request.
    /// </summary>
    /// <returns>A validated pagination request.</returns>
    public PaginationRequest Validate()
    {
        if (Page < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Page),
                "Page must be greater than or equal to 1.");
        }

        if (PageSize < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(PageSize),
                "Page size must be greater than or equal to 1.");
        }

        if (PageSize > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(PageSize),
                "Page size cannot be greater than 100.");
        }

        return this;
    }
}