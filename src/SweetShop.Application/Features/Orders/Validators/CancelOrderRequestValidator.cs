using FluentValidation;
using SweetShop.Application.Features.Orders.Requests;

namespace SweetShop.Application.Features.Orders.Validators;

/// <summary>
/// Validates requests that cancel customer orders.
/// </summary>
public sealed class CancelOrderRequestValidator
    : AbstractValidator<CancelOrderRequest>
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="CancelOrderRequestValidator"/> class.
    /// </summary>
    public CancelOrderRequestValidator()
    {
        RuleFor(request => request.Reason)
            .NotEmpty()
            .WithMessage("Cancellation reason is required.")
            .MaximumLength(500)
            .WithMessage(
                "Cancellation reason cannot exceed 500 characters.");
    }
}