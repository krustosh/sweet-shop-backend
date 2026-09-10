using FluentValidation;
using SweetShop.Application.Authentication.Requests;

namespace SweetShop.Application.Authentication.Validators;

/// <summary>
/// Represents a validator for the <see cref="RequestOtpRequest"/> class, ensuring that the mobile number provided is valid and meets the required criteria.
/// </summary>
public sealed class RequestOtpRequestValidator : AbstractValidator<RequestOtpRequest>
{

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestOtpRequestValidator"/> class, setting up validation rules for the <see cref="RequestOtpRequest"/>.
    /// </summary>
    public RequestOtpRequestValidator()
    {
        RuleFor(x => x.MobileNumber)
            .NotEmpty()
            .WithMessage("Mobile number is required.")
            .Must(BeValidIndianMobileNumber)
            .WithMessage("Mobile number must be a valid Indian mobile number.");
    }

    private static bool BeValidIndianMobileNumber(string? mobileNumber)
    {
        if (string.IsNullOrWhiteSpace(mobileNumber))
        {
            return false;
        }

        var normalized = mobileNumber.Trim();

        if (normalized.StartsWith("+91", StringComparison.Ordinal))
        {
            normalized = normalized[3..];
        }
        else if (normalized.StartsWith("91", StringComparison.Ordinal) &&
                 normalized.Length == 12)
        {
            normalized = normalized[2..];
        }

        return normalized.Length == 10 &&
               normalized.All(char.IsDigit) &&
               normalized[0] is >= '6' and <= '9';
    }
}