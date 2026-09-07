namespace SweetShop.Application.Common;

/// <summary>
/// Represents a structured application error.
/// </summary>
public sealed record ApplicationError(
    string Code,
    string Message);