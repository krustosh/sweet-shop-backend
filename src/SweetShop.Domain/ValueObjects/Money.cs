namespace SweetShop.Domain.ValueObjects;

/// <summary>
/// Represents a monetary amount in a specific currency.
/// </summary>
public sealed record Money
{
    /// <summary>
    /// Gets the monetary amount.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Gets the ISO 4217 currency code.
    /// </summary>
    public string Currency { get; }

    /// <summary>
    /// Initializes a new monetary value.
    /// </summary>
    /// <param name="amount">The monetary amount.</param>
    /// <param name="currency">The ISO 4217 currency code.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the amount is negative.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the currency is invalid.
    /// </exception>
    public Money(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Money amount cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException(
                "Currency is required.",
                nameof(currency));
        }

        Currency = currency.Trim().ToUpperInvariant();
        Amount = decimal.Round(
            amount,
            2,
            MidpointRounding.AwayFromZero);
    }
}