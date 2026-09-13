using FluentValidation;
using SweetShop.Application.Features.Categories.Requests;

namespace SweetShop.Application.Features.Categories.Validators;

/// <summary>
/// Validates requests used to create product categories.
/// </summary>
public sealed class CreateCategoryRequestValidator
    : AbstractValidator<CreateCategoryRequest>
{
    /// <summary>
    /// Initializes validation rules for category creation.
    /// </summary>
    public CreateCategoryRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Description)
            .MaximumLength(1000)
            .When(request => request.Description is not null);

        RuleFor(request => request.ImageUrl)
            .MaximumLength(2048)
            .When(request => request.ImageUrl is not null);

        RuleFor(request => request.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}