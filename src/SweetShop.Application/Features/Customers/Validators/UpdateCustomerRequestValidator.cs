using FluentValidation;
using SweetShop.Application.Features.Customers.Requests;

namespace SweetShop.Application.Features.Customers.Validators;

/// <summary>
/// Represents a validator for the <see cref="UpdateCustomerRequest"/> class, ensuring that the request data meets the required validation rules.
/// </summary>
public sealed class UpdateCustomerRequestValidator
    : AbstractValidator<UpdateCustomerRequest>
{

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCustomerRequestValidator"/> class and defines the validation rules for the <see cref="UpdateCustomerRequest"/> properties.
    /// </summary>
    public UpdateCustomerRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Email)
            .EmailAddress()
            .MaximumLength(320)
            .When(request => !string.IsNullOrWhiteSpace(request.Email));

        RuleFor(request => request.Notes)
            .MaximumLength(2000)
            .When(request => !string.IsNullOrWhiteSpace(request.Notes));
    }
}