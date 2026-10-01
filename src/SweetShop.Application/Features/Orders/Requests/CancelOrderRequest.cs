namespace SweetShop.Application.Features.Orders.Requests;

/// <summary>
/// Represents a request to cancel an order.
/// </summary>
/// <param name="Reason">The reason supplied for cancellation.</param>
public sealed record CancelOrderRequest(
    string Reason);