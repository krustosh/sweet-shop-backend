using FluentValidation;

namespace SweetShop.Application.Features.ProductImages.Validators;

/// <summary>
/// Validates product image creation requests.
/// </summary>
public sealed class CreateProductImageRequestValidator
    : AbstractValidator<Requests.CreateProductImageRequest>
{
    /// <summary>
    /// 
    /// </summary>
    public CreateProductImageRequestValidator()
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