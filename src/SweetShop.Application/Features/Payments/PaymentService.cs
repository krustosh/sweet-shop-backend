using SweetShop.Application.Features.Customers;
using SweetShop.Application.Features.Payments.Requests;
using SweetShop.Application.Features.Payments.Responses;
using SweetShop.Application.Interfaces;
using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;

namespace SweetShop.Application.Features.Payments;

/// <summary>
/// Provides customer-facing payment management operations.
/// </summary>
public sealed class PaymentService : IPaymentService
{
    private readonly ICurrentUser currentUser;
    private readonly ICustomerStore customerStore;
    private readonly IPaymentStore paymentStore;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentService"/> class.
    /// </summary>
    /// <param name="currentUser">The authenticated user context.</param>
    /// <param name="customerStore">The customer persistence abstraction.</param>
    /// <param name="paymentStore">The payment persistence abstraction.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public PaymentService(
        ICurrentUser currentUser,
        ICustomerStore customerStore,
        IPaymentStore paymentStore,
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(currentUser);
        ArgumentNullException.ThrowIfNull(customerStore);
        ArgumentNullException.ThrowIfNull(paymentStore);
        ArgumentNullException.ThrowIfNull(unitOfWork);

        this.currentUser = currentUser;
        this.customerStore = customerStore;
        this.paymentStore = paymentStore;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<PaymentResponse> InitiateAsync(
        Guid orderId,
        InitiatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        ValidateId(orderId, nameof(orderId));
        ArgumentNullException.ThrowIfNull(request);

        if (!Enum.TryParse<PaymentMethod>(
                request.Method,
                ignoreCase: true,
                out var method)
            || !Enum.IsDefined(method))
        {
            throw new ArgumentException(
                "Payment method must be Upi or Cash.",
                nameof(request));
        }

        var customer = await GetCurrentCustomerAsync(cancellationToken);

        var order = await paymentStore.GetCustomerOrderAsync(
            customer.Id,
            orderId,
            cancellationToken);

        if (order is null)
        {
            throw new KeyNotFoundException(
                "The specified order was not found.");
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Payment cannot be initiated for a cancelled order.");
        }

        if (order.PaymentStatus == PaymentStatus.Success)
        {
            throw new InvalidOperationException(
                "The order has already been paid.");
        }

        if (method == PaymentMethod.Upi)
        {
            throw new InvalidOperationException(
                "UPI payment initiation is unavailable until a payment provider is configured.");
        }

        var payment = new Payment(
            order.Id,
            GeneratePaymentNumber(),
            method,
            order.TotalAmount);

        paymentStore.Add(payment);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(payment);
    }

    /// <inheritdoc />
    public async Task<PaymentResponse?> GetMyPaymentAsync(
        Guid paymentId,
        CancellationToken cancellationToken)
    {
        ValidateId(paymentId, nameof(paymentId));

        var customer = await GetCurrentCustomerAsync(cancellationToken);

        var payment = await paymentStore.GetCustomerPaymentAsync(
            customer.Id,
            paymentId,
            cancellationToken);

        return payment is null ? null : Map(payment);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<PaymentResponse>?> GetMyOrderPaymentsAsync(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        ValidateId(orderId, nameof(orderId));

        var customer = await GetCurrentCustomerAsync(cancellationToken);

        var order = await paymentStore.GetCustomerOrderAsync(
            customer.Id,
            orderId,
            cancellationToken);

        if (order is null)
        {
            return null;
        }

        var payments = await paymentStore.GetByOrderIdAsync(
            order.Id,
            cancellationToken);

        return payments.Select(Map).ToArray();
    }

    private async Task<Customer> GetCurrentCustomerAsync(
        CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "Authenticated user ID was not found.");
        }

        var customer = await customerStore.GetByUserIdAsync(
            currentUser.UserId.Value,
            cancellationToken);

        return customer ?? throw new InvalidOperationException(
            "Authenticated user is not associated with a customer.");
    }

    private static PaymentResponse Map(Payment payment)
    {
        return new PaymentResponse
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            PaymentNumber = payment.PaymentNumber,
            Method = payment.Method.ToString(),
            Status = payment.Status.ToString(),
            Amount = payment.Amount.Amount,
            Currency = payment.Amount.Currency,
            Provider = payment.Provider,
            ProviderTransactionId = payment.ProviderTransactionId,
            PaidAt = payment.PaidAt,
            CreatedAt = payment.CreatedAt
        };
    }

    private static string GeneratePaymentNumber()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];

        return $"PAY-{DateTime.UtcNow:yyyyMMddHHmmss}-{suffix}";
    }

    private static void ValidateId(Guid id, string parameterName)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "Identifier cannot be empty.",
                parameterName);
        }
    }
}
