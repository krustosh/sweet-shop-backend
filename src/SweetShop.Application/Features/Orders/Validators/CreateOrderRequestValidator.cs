using FluentValidation;
using SweetShop.Application.Features.Orders.Requests;
using SweetShop.Domain.Enums;

namespace SweetShop.Application.Features.Orders.Validators;

/// <summary>
/// Validates requests that create customer orders.
/// </summary>
public sealed class CreateOrderRequestValidator
    : AbstractValidator<CreateOrderRequest>
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="CreateOrderRequestValidator"/> class.
    /// </summary>
    public CreateOrderRequestValidator()
    {
        RuleFor(request => request.FulfillmentType)
            .IsInEnum()
            .WithMessage("A valid fulfillment type is required.");

        RuleFor(request => request.DeliveryAddressId)
            .NotEmpty()
            .When(request =>
                request.FulfillmentType == FulfillmentType.Delivery)
            .WithMessage(
                "A delivery address is required for delivery orders.");

        RuleFor(request => request.DeliveryAddressId)
            .Empty()
            .When(request =>
                request.FulfillmentType == FulfillmentType.Pickup)
            .WithMessage(
                "A delivery address must not be supplied for pickup orders.");

        RuleFor(request => request.CustomerNote)
            .MaximumLength(500)
            .WithMessage(
                "Customer note cannot exceed 500 characters.");
    }
}