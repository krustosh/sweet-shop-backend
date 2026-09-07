using SweetShop.Domain.Common;

namespace SweetShop.Domain.Entities;

/// <summary>
/// Represents the active shopping cart of a customer.
/// </summary>
public sealed class Cart : AuditableEntity
{
    private readonly List<CartItem> _items = [];

    /// <summary>
    /// Gets the identifier of the customer who owns the cart.
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// Gets the items currently in the cart.
    /// </summary>
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    /// <summary>
    /// Initializes a new shopping cart.
    /// </summary>
    /// <param name="customerId">The identifier of the cart owner.</param>
    public Cart(Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Customer ID cannot be empty.",
                nameof(customerId));
        }

        CustomerId = customerId;
    }

    /// <summary>
    /// Adds an item to the cart.
    /// </summary>
    /// <param name="item">The cart item to add.</param>
    public void AddItem(CartItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (item.CartId != Id)
        {
            throw new ArgumentException(
                "Cart item belongs to a different cart.",
                nameof(item));
        }

        _items.Add(item);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes an item from the cart.
    /// </summary>
    /// <param name="itemId">The identifier of the item to remove.</param>
    public void RemoveItem(Guid itemId)
    {
        if (itemId == Guid.Empty)
        {
            throw new ArgumentException(
                "Cart item ID cannot be empty.",
                nameof(itemId));
        }

        var item = _items.FirstOrDefault(item => item.Id == itemId);

        if (item is null)
        {
            throw new InvalidOperationException(
                "Cart item was not found.");
        }

        _items.Remove(item);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Clears all items from the cart.
    /// </summary>
    public void Clear()
    {
        _items.Clear();
        UpdatedAt = DateTime.UtcNow;
    }
}