using FluentValidation;

namespace SweetShop.Application.Features.ProductImages.Validators;

/// <summary>
/// Validates product image update requests.
/// </summary>
public sealed class UpdateProductImageRequestValidator
    : AbstractValidator<Requests.UpdateProductImageRequest>
{
    /// <summary>
    /// 
    /// </summary>
    public UpdateProductImageRequestValidator()
    {
        RuleFor(request => request.Url)
            .NotEmpty()
            .MaximumLength(2048);

        RuleFor(request => request.AltText)
            .MaximumLength(500);

        RuleFor(request => request.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}