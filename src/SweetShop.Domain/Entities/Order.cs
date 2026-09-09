using SweetShop.Domain.Common;
using SweetShop.Domain.Enums;
using SweetShop.Domain.ValueObjects;

namespace SweetShop.Domain.Entities;

/// <summary>
/// Represents a customer order.
/// </summary>
public sealed class Order : AuditableEntity
{
    private readonly List<OrderItem> _items = [];

    /// <summary>
    /// Gets the human-readable order number.
    /// </summary>
    public string OrderNumber { get; private set; }

    /// <summary>
    /// Gets the identifier of the customer who placed the order.
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// Gets the fulfillment method for the order.
    /// </summary>
    public FulfillmentType FulfillmentType { get; private set; }

    /// <summary>
    /// Gets the current order status.
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// Gets the current payment status.
    /// </summary>
    public PaymentStatus PaymentStatus { get; private set; }

    /// <summary>
    /// Gets the order subtotal.
    /// </summary>
    public Money Subtotal { get; private set; }

    /// <summary>
    /// Gets the delivery fee.
    /// </summary>
    public Money DeliveryFee { get; private set; }

    /// <summary>
    /// Gets the discount amount.
    /// </summary>
    public Money DiscountAmount { get; private set; }

    /// <summary>
    /// Gets the final order total.
    /// </summary>
    public Money TotalAmount { get; private set; }

    /// <summary>
    /// Gets the optional customer note.
    /// </summary>
    public string? CustomerNote { get; private set; }

    /// <summary>
    /// Gets the immutable delivery address captured when the order was placed.
    /// </summary>
    public DeliveryAddressSnapshot? DeliveryAddressSnapshot { get; private set; }

    /// <summary>
    /// Gets the optional pickup details.
    /// </summary>
    public string? PickupDetails { get; private set; }

    /// <summary>
    /// Gets the date and time when the order was placed.
    /// </summary>
    public DateTime? PlacedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the order was accepted.
    /// </summary>
    public DateTime? AcceptedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when preparation started.
    /// </summary>
    public DateTime? PreparingAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the order became ready.
    /// </summary>
    public DateTime? ReadyAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the order was completed.
    /// </summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the order was cancelled.
    /// </summary>
    public DateTime? CancelledAt { get; private set; }

    /// <summary>
    /// Gets the reason for cancellation.
    /// </summary>
    public string? CancellationReason { get; private set; }

    /// <summary>
    /// Gets the items belonging to the order.
    /// </summary>
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    
    private Order()
    {
        OrderNumber = string.Empty;
        Subtotal = new Money(0, DomainConstants.CurrencyInr);
        DeliveryFee = new Money(0, DomainConstants.CurrencyInr);
        DiscountAmount = new Money(0, DomainConstants.CurrencyInr);
        TotalAmount = new Money(0, DomainConstants.CurrencyInr);
        }

    /// <summary>
    /// Initializes a new order.
    /// </summary>
    /// <param name="orderNumber">The human-readable order number.</param>
    /// <param name="customerId">The identifier of the customer.</param>
    /// <param name="fulfillmentType">The fulfillment method.</param>
    /// <param name="deliveryFee">The delivery fee.</param>
    /// <param name="discountAmount">The discount amount.</param>
    /// <param name="customerNote">The optional customer note.</param>
    /// <param name="deliveryAddressSnapshot">
    /// The immutable delivery address snapshot.
    /// </param>
    /// <param name="pickupDetails">The optional pickup details.</param>
    public Order(
        string orderNumber,
        Guid customerId,
        FulfillmentType fulfillmentType,
        Money deliveryFee,
        Money discountAmount,
        string? customerNote,
        DeliveryAddressSnapshot? deliveryAddressSnapshot,
        string? pickupDetails)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Customer ID cannot be empty.",
                nameof(customerId));
        }

        ArgumentNullException.ThrowIfNull(deliveryFee);
        ArgumentNullException.ThrowIfNull(discountAmount);

        if (fulfillmentType == FulfillmentType.Delivery &&
            deliveryAddressSnapshot is null)
        {
            throw new ArgumentException(
                "A delivery address is required for delivery orders.",
                nameof(deliveryAddressSnapshot));
        }

        if (fulfillmentType == FulfillmentType.Pickup &&
            deliveryAddressSnapshot is not null)
        {
            throw new ArgumentException(
                "A delivery address cannot be supplied for pickup orders.",
                nameof(deliveryAddressSnapshot));
        }

        OrderNumber = RequireValue(orderNumber, nameof(orderNumber));
        CustomerId = customerId;
        FulfillmentType = fulfillmentType;
        Status = OrderStatus.Placed;
        PaymentStatus = PaymentStatus.Pending;
        DeliveryFee = deliveryFee;
        DiscountAmount = discountAmount;
        CustomerNote = NormalizeOptional(customerNote);
        DeliveryAddressSnapshot = deliveryAddressSnapshot;
        PickupDetails = NormalizeOptional(pickupDetails);
        PlacedAt = DateTime.UtcNow;

        Subtotal = ZeroMoney(deliveryFee.Currency);
        TotalAmount = CalculateTotal(
            Subtotal,
            DeliveryFee,
            DiscountAmount);
    }

    /// <summary>
    /// Adds an item to the order.
    /// </summary>
    /// <param name="item">The order item to add.</param>
    public void AddItem(OrderItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (Status != OrderStatus.Placed)
        {
            throw new InvalidOperationException(
                "Items can only be added while the order is in the placed state.");
        }

        if (item.OrderId != Id)
        {
            throw new ArgumentException(
                "Order item belongs to a different order.",
                nameof(item));
        }

        _items.Add(item);
        RecalculateTotals();
    }

    /// <summary>
    /// Accepts the order.
    /// </summary>
    public void Accept()
    {
        EnsureStatus(OrderStatus.Placed);

        Status = OrderStatus.Accepted;
        AcceptedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Starts preparing the order.
    /// </summary>
    public void StartPreparing()
    {
        EnsureStatus(OrderStatus.Accepted);

        Status = OrderStatus.Preparing;
        PreparingAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the order as ready.
    /// </summary>
    public void MarkReady()
    {
        EnsureStatus(OrderStatus.Preparing);

        Status = OrderStatus.Ready;
        ReadyAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks a delivery order as out for delivery.
    /// </summary>
    public void MarkOutForDelivery()
    {
        EnsureStatus(OrderStatus.Ready);

        if (FulfillmentType != FulfillmentType.Delivery)
        {
            throw new InvalidOperationException(
                "Only delivery orders can be marked out for delivery.");
        }

        Status = OrderStatus.OutForDelivery;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks a delivery order as delivered.
    /// </summary>
    public void MarkDelivered()
    {
        EnsureStatus(OrderStatus.OutForDelivery);

        Status = OrderStatus.Delivered;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks a pickup order as picked up.
    /// </summary>
    public void MarkPickedUp()
    {
        EnsureStatus(OrderStatus.Ready);

        if (FulfillmentType != FulfillmentType.Pickup)
        {
            throw new InvalidOperationException(
                "Only pickup orders can be marked as picked up.");
        }

        Status = OrderStatus.PickedUp;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Completes an order after fulfillment.
    /// </summary>
    public void Complete()
    {
        if (Status is not (OrderStatus.Delivered or OrderStatus.PickedUp))
        {
            throw new InvalidOperationException(
                "Only delivered or picked-up orders can be completed.");
        }

        Status = OrderStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the order.
    /// </summary>
    /// <param name="reason">The cancellation reason.</param>
    public void Cancel(string reason)
    {
        if (Status is OrderStatus.Completed or OrderStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Completed or already cancelled orders cannot be cancelled.");
        }

        CancellationReason = RequireValue(reason, nameof(reason));
        Status = OrderStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the payment status of the order.
    /// </summary>
    /// <param name="paymentStatus">The new payment status.</param>
    public void UpdatePaymentStatus(PaymentStatus paymentStatus)
    {
        PaymentStatus = paymentStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    private void RecalculateTotals()
    {
        var subtotalAmount = _items.Sum(item => item.LineTotal.Amount);

        Subtotal = new Money(
            subtotalAmount,
            DeliveryFee.Currency);

        TotalAmount = CalculateTotal(
            Subtotal,
            DeliveryFee,
            DiscountAmount);

        UpdatedAt = DateTime.UtcNow;
    }

    private static Money CalculateTotal(
        Money subtotal,
        Money deliveryFee,
        Money discountAmount)
    {
        if (subtotal.Currency != deliveryFee.Currency ||
            subtotal.Currency != discountAmount.Currency)
        {
            throw new InvalidOperationException(
                "All order monetary values must use the same currency.");
        }

        var total = subtotal.Amount +
                    deliveryFee.Amount -
                    discountAmount.Amount;

        if (total < 0)
        {
            throw new InvalidOperationException(
                "Order total cannot be negative.");
        }

        return new Money(total, subtotal.Currency);
    }

    private void EnsureStatus(OrderStatus expectedStatus)
    {
        if (Status != expectedStatus)
        {
            throw new InvalidOperationException(
                $"Order must be in {expectedStatus} status.");
        }
    }

    private static Money ZeroMoney(string currency)
    {
        return new Money(0, currency);
    }

    private static string RequireValue(
        string value,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Value is required.",
                parameterName);
        }

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}