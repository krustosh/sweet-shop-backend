using Microsoft.EntityFrameworkCore;
using SweetShop.Application.Features.Payments;
using SweetShop.Domain.Entities;
using SweetShop.Infrastructure.Persistence.Context;

namespace SweetShop.Infrastructure.Persistence.Stores;

/// <summary>
/// Provides database persistence operations for payment management.
/// </summary>
public sealed class PaymentStore : IPaymentStore
{
    private readonly SweetShopDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentStore"/> class.
    /// </summary>
    /// <param name="dbContext">The application's database context.</param>
    public PaymentStore(SweetShopDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<Order?> GetCustomerOrderAsync(
        Guid customerId,
        Guid orderId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Order>()
            .SingleOrDefaultAsync(
                order => order.Id == orderId
                    && order.CustomerId == customerId,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Payment?> GetCustomerPaymentAsync(
        Guid customerId,
        Guid paymentId,
        CancellationToken cancellationToken)
    {
        return await (
            from payment in _dbContext.Set<Payment>()
            join order in _dbContext.Set<Order>()
                on payment.OrderId equals order.Id
            where payment.Id == paymentId
                && order.CustomerId == customerId
            select payment)
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Payment>> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Payment>()
            .Where(payment => payment.OrderId == orderId)
            .OrderByDescending(payment => payment.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Add(Payment payment)
    {
        ArgumentNullException.ThrowIfNull(payment);

        _dbContext.Set<Payment>().Add(payment);
    }
}
