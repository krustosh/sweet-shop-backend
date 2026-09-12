using FluentValidation;
using SweetShop.Application.Features.Addresses.Requests;

namespace SweetShop.Application.Features.Addresses.Validators;

/// <summary>
/// Validator for the <see cref="UpdateAddressRequest"/> class.
/// </summary>
public sealed class UpdateAddressRequestValidator
    : AbstractValidator<UpdateAddressRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateAddressRequestValidator"/> class.
    /// </summary>
    public UpdateAddressRequestValidator()
    {
        RuleFor(request => request.Label)
            .IsInEnum()
            .WithMessage("Address label is invalid.");

        RuleFor(request => request.RecipientName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.MobileNumber)
            .Matches("^[6-9][0-9]{9}$")
            .WithMessage("Mobile number must be a valid Indian 10-digit mobile number.");

        RuleFor(request => request.AddressLine1)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(request => request.AddressLine2)
            .MaximumLength(500);

        RuleFor(request => request.Landmark)
            .MaximumLength(300);

        RuleFor(request => request.Area)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.State)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.PostalCode)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(request => request.Latitude)
            .InclusiveBetween(-90m, 90m)
            .When(request => request.Latitude.HasValue);

        RuleFor(request => request.Longitude)
            .InclusiveBetween(-180m, 180m)
            .When(request => request.Longitude.HasValue);
    }
}