using FluentValidation;

namespace SweetShop.Application.Features.Inventory.Validators;

/// <summary>
/// Validates inventory creation requests.
/// </summary>
public sealed class CreateInventoryRequestValidator
    : AbstractValidator<Requests.CreateInventoryRequest>
{
    /// <summary>
    /// 
    /// </summary>
    public CreateInventoryRequestValidator()
    {
        RuleFor(request => request.LowStockThreshold)
            .GreaterThanOrEqualTo(0);
    }
}