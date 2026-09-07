namespace SweetShop.Application.Common;

/// <summary>
/// Defines validation behavior for an application request.
/// </summary>
/// <typeparam name="TRequest">The request type to validate.</typeparam>
public interface IValidator<in TRequest>
{
    /// <summary>
    /// Validates the specified request.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <returns>The validation errors. An empty collection indicates a valid request.</returns>
    IReadOnlyCollection<ApplicationError> Validate(TRequest request);
}