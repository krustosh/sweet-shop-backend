namespace SweetShop.Api.Common.Models;

/// <summary>
/// Represents the details of an API error, including the field that caused the error and a descriptive message.
/// </summary>
/// <param name="Field">The field that caused the error.</param>
/// <param name="Message">The descriptive error message.</param>
public sealed record ApiErrorDetail(
    string Field,
    string Message);