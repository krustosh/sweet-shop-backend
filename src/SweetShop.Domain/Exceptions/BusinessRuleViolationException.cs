namespace SweetShop.Domain.Exceptions;

/// <summary>
/// Represents a specific violation of a business rule within the domain.
/// </summary>
public sealed class BusinessRuleViolationException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessRuleViolationException"/> class.
    /// </summary>
    /// <param name="message">The business-rule violation message.</param>
    public BusinessRuleViolationException(string message)
        : base(message)
    {
    }
}