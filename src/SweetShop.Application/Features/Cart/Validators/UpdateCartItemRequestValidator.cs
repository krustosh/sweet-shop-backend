using FluentValidation;
using SweetShop.Application.Features.Cart.Requests;

namespace SweetShop.Application.Features.Cart.Validators;

/// <summary>
/// Validates requests that update cart-item quantities.
/// </summary>
public sealed class UpdateCartItemRequestValidator
    : AbstractValidator<UpdateCartItemRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCartItemRequestValidator"/> class.
    /// </summary>
    public UpdateCartItemRequestValidator()
    {
        RuleFor(request => request.Quantity)
            .GreaterThan(0)
            .WithMessage("Cart item quantity must be greater than zero.");
    }
}
