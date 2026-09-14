using FluentValidation;
using SweetShop.Application.Features.Products.Requests;

namespace SweetShop.Application.Features.Products.Validators;

/// <summary>
/// Validates product update requests.
/// </summary>
public sealed class UpdateProductRequestValidator
    : AbstractValidator<UpdateProductRequest>
{
    /// <summary>
    /// 
    /// </summary>
    public UpdateProductRequestValidator()
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