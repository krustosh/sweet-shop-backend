using FluentValidation;

namespace SweetShop.Application.Features.Payments.Requests;

/// <summary>
/// Validates payment initiation requests.
/// </summary>
public sealed class InitiatePaymentRequestValidator
    : AbstractValidator<InitiatePaymentRequest>
{
    /// <summary>
    /// Initializes the payment initiation request validation rules.
    /// </summary>
    public InitiatePaymentRequestValidator()
    {
        RuleFor(request => request.Method)
            .NotEmpty()
            .Must(method =>
                string.Equals(method, "Upi", StringComparison.OrdinalIgnoreCase)
                || string.Equals(method, "Cash", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Payment method must be Upi or Cash.");
    }
}