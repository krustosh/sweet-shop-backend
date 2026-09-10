namespace SweetShop.Api.Common.Models;

/// <summary>
/// Represents the response structure for a successful API operation, encapsulating the data in a standardized format.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="Data"></param>
public sealed record ApiResponse<T>(
    T Data);