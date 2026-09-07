namespace SweetShop.Domain.Exceptions;

/// <summary>
/// Represents a violation of a domain business rule.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class.
    /// </summary>
    /// <param name="message">The business-rule violation message.</param>
    public DomainException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class.
    /// </summary>
    /// <param name="message">The business-rule violation message.</param>
    /// <param name="innerException">The exception that caused this exception.</param>
    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}