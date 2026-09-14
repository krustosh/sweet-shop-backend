using FluentValidation;
using SweetShop.Application.Features.Products.Requests;

namespace SweetShop.Application.Features.Products.Validators;

/// <summary>
/// Validates product variant creation requests.
/// </summary>
public sealed class CreateProductVariantRequestValidator
    : AbstractValidator<CreateProductVariantRequest>
{
    /// <summary>
    /// 
    /// </summary>
    public CreateProductVariantRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Quantity)
            .GreaterThan(0);

        RuleFor(request => request.Unit)
            .IsInEnum();

        RuleFor(request => request.PriceAmount)
            .GreaterThan(0)
            .PrecisionScale(18, 2, false);

        RuleFor(request => request.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}