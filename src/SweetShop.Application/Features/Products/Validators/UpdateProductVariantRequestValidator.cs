using FluentValidation;
using SweetShop.Application.Features.Products.Requests;

namespace SweetShop.Application.Features.Products.Validators;

/// <summary>
/// Validates product variant update requests.
/// </summary>
public sealed class UpdateProductVariantRequestValidator
    : AbstractValidator<UpdateProductVariantRequest>
{
    /// <summary>
    /// 
    /// </summary>
    public UpdateProductVariantRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.PriceAmount)
            .GreaterThan(0)
            .PrecisionScale(18, 2, false);

        RuleFor(request => request.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}