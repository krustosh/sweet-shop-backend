using FluentValidation;
using SweetShop.Application.Features.Cart.Requests;

namespace SweetShop.Application.Features.Cart.Validators;

/// <summary>
/// Validates requests that add items to a cart.
/// </summary>
public sealed class AddCartItemRequestValidator
    : AbstractValidator<AddCartItemRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddCartItemRequestValidator"/> class.
    /// </summary>
    public AddCartItemRequestValidator()
    {
        RuleFor(request => request.ProductVariantId)
            .NotEmpty()
            .WithMessage("Product variant ID is required.");

        RuleFor(request => request.Quantity)
            .GreaterThan(0)
            .WithMessage("Cart item quantity must be greater than zero.");
    }
}
