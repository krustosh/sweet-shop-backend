using FluentValidation;
using SweetShop.Application.Features.Products.Requests;

namespace SweetShop.Application.Features.Products.Validators;

/// <summary>
/// Validates product creation requests.
/// </summary>
public sealed class CreateProductRequestValidator
    : AbstractValidator<CreateProductRequest>
{
    /// <summary>
    /// 
    /// </summary>
    public CreateProductRequestValidator()
    {
        RuleFor(request => request.CategoryId)
            .NotEmpty();

        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Description)
            .MaximumLength(2000)
            .When(request => request.Description is not null);

        RuleFor(request => request.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}