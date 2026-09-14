using FluentValidation;

namespace SweetShop.Application.Features.Inventory.Validators;

/// <summary>
/// 
/// </summary>
public sealed class CreateInventoryTransactionRequestValidator
    : AbstractValidator<Requests.CreateInventoryTransactionRequest>
{
    /// <summary>
    /// 
    /// </summary>
    public CreateInventoryTransactionRequestValidator()
    {
        RuleFor(request => request.Quantity)
            .GreaterThan(0);

        RuleFor(request => request.Type)
            .IsInEnum();

        RuleFor(request => request.ReferenceType)
            .MaximumLength(100);

        RuleFor(request => request.Reason)
            .MaximumLength(1000);

        RuleFor(request => request.ReferenceId)
            .Must(referenceId =>
                !referenceId.HasValue ||
                referenceId.Value != Guid.Empty)
            .WithMessage("Reference ID cannot be empty.");
    }
}