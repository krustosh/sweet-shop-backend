namespace SweetShop.Application.Interfaces;

/// <summary>
/// Provides access to the shop context for the current application operation.
/// </summary>
public interface IShopContext
{
    /// <summary>
    /// Gets the identifier of the shop associated with the current operation.
    /// </summary>
    Guid ShopId { get; }
}