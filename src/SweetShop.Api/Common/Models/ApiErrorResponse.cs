namespace SweetShop.Api.Common.Models;

/// <summary>
/// Represents the response structure for an API error, encapsulating the error details in a standardized format.
/// </summary>
/// <param name="Error">The API error details.</param>
public sealed record ApiErrorResponse(
    ApiError Error);

/// <summary>
/// Represents the structure of an API error, including a code, message, and detailed information.
/// </summary>
/// <param name="Code">The error code.</param>
/// <param name="Message">The error message.</param>
/// <param name="Details">The detailed error information.</param>
public sealed record ApiError(
    string Code,
    string Message,
    IReadOnlyCollection<ApiErrorDetail> Details);