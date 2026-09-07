namespace SweetShop.Application.Common;

/// <summary>
/// Represents the outcome of application request validation.
/// </summary>
public sealed record ValidationResult(
    bool IsValid,
    IReadOnlyCollection<ApplicationError> Errors)
{
    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    /// <returns>A valid validation result.</returns>
    public static ValidationResult Valid()
    {
        return new ValidationResult(true, []);
    }

    /// <summary>
    /// Creates a failed validation result.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    /// <returns>An invalid validation result.</returns>
    public static ValidationResult Invalid(
        IReadOnlyCollection<ApplicationError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        if (errors.Count == 0)
        {
            throw new ArgumentException(
                "At least one validation error is required.",
                nameof(errors));
        }

        return new ValidationResult(false, errors);
    }
}