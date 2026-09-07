namespace SweetShop.Application.Common;

/// <summary>
/// Represents the result of an application operation.
/// </summary>
/// <typeparam name="T">The type of the operation result.</typeparam>
public sealed class Result<T>
{
    /// <summary>
    /// Initializes a new successful result.
    /// </summary>
    /// <param name="value">The successful operation value.</param>
    /// <returns>A successful result.</returns>
    public Result(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        IsSuccess = true;
        Value = value;
        Errors = [];
    }

    /// <summary>
    /// Initializes a new failed result.
    /// </summary>
    /// <param name="errors">The errors produced by the operation.</param>
    /// <returns>A failed result.</returns>
    public Result(IReadOnlyCollection<ApplicationError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        if (errors.Count == 0)
        {
            throw new ArgumentException(
                "At least one error is required.",
                nameof(errors));
        }

        IsSuccess = false;
        Value = default;
        Errors = errors;
    }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the operation result value.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Gets the errors produced by the operation.
    /// </summary>
    public IReadOnlyCollection<ApplicationError> Errors { get; }
}