
using SweetShop.Application.Features.Customers;
using SweetShop.Application.Features.Payments;
using SweetShop.Application.Features.Payments.Requests;
using SweetShop.Application.Interfaces;
using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;
using SweetShop.Domain.ValueObjects;

namespace SweetShop.UnitTests.Features.Payments;

/// <summary>
/// Unit tests for the <see cref="PaymentService"/> class.
/// </summary>
public sealed class PaymentServiceTests
{
    /// <summary>
    /// Tests that cash payment initiation creates a pending payment
    /// using the saved order total.
    /// </summary>
    [Fact]
    public async Task InitiateCashPaymentCreatesPendingPaymentUsingOrderTotal()
    {
        var userId = Guid.NewGuid();
        var customer = CreateCustomer(userId);
        var order = CreateOrder(customer.Id);
        var paymentStore = new FakePaymentStore(order);
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            userId,
            customer,
            paymentStore,
            unitOfWork);

        var result = await service.InitiateAsync(
            order.Id,
            new InitiatePaymentRequest { Method = "Cash" },
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(order.Id, result.OrderId);
        Assert.Equal("Cash", result.Method);
        Assert.Equal("Pending", result.Status);
        Assert.Equal(order.TotalAmount.Amount, result.Amount);
        Assert.Equal(order.TotalAmount.Currency, result.Currency);
        Assert.NotNull(paymentStore.AddedPayment);
        Assert.Equal(order.Id, paymentStore.AddedPayment.OrderId);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    /// <summary>
    /// Tests that UPI payment initiation is rejected until a payment
    /// provider has been configured.
    /// </summary>
    [Fact]
    public async Task InitiateUpiPaymentWhenProviderIsNotConfiguredThrows()
    {
        var userId = Guid.NewGuid();
        var customer = CreateCustomer(userId);
        var order = CreateOrder(customer.Id);
        var paymentStore = new FakePaymentStore(order);
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            userId,
            customer,
            paymentStore,
            unitOfWork);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.InitiateAsync(
                order.Id,
                new InitiatePaymentRequest { Method = "Upi" },
                CancellationToken.None));

        Assert.Contains("payment provider is configured", exception.Message);
        Assert.Null(paymentStore.AddedPayment);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    /// <summary>
    /// Tests that a customer cannot initiate payment for an order
    /// that does not belong to them.
    /// </summary>
    [Fact]
    public async Task InitiatePaymentForAnotherCustomersOrderThrows()
    {
        var userId = Guid.NewGuid();
        var customer = CreateCustomer(userId);
        var anotherCustomer = CreateCustomer(Guid.NewGuid());
        var anotherCustomersOrder = CreateOrder(anotherCustomer.Id);
        var paymentStore = new FakePaymentStore(anotherCustomersOrder);
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            userId,
            customer,
            paymentStore,
            unitOfWork);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.InitiateAsync(
                anotherCustomersOrder.Id,
                new InitiatePaymentRequest { Method = "Cash" },
                CancellationToken.None));

        Assert.Null(paymentStore.AddedPayment);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    /// <summary>
    /// Tests that a cancelled order cannot receive a new payment.
    /// </summary>
    [Fact]
    public async Task InitiatePaymentForCancelledOrderThrows()
    {
        var userId = Guid.NewGuid();
        var customer = CreateCustomer(userId);
        var order = CreateOrder(customer.Id);
        order.Cancel("Customer requested cancellation");

        var paymentStore = new FakePaymentStore(order);
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            userId,
            customer,
            paymentStore,
            unitOfWork);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.InitiateAsync(
                order.Id,
                new InitiatePaymentRequest { Method = "Cash" },
                CancellationToken.None));

        Assert.Null(paymentStore.AddedPayment);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    /// <summary>
    /// Tests that an unauthenticated user cannot initiate a payment.
    /// </summary>
    [Fact]
    public async Task InitiatePaymentWhenUserIsUnauthenticatedThrows()
    {
        var service = new PaymentService(
            new FakeCurrentUser(null),
            new FakeCustomerStore(),
            new FakePaymentStore(),
            new FakeUnitOfWork());

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.InitiateAsync(
                Guid.NewGuid(),
                new InitiatePaymentRequest { Method = "Cash" },
                CancellationToken.None));

        Assert.Equal(
            "Authenticated user ID was not found.",
            exception.Message);
    }

    /// <summary>
    /// Tests that a customer can retrieve a payment belonging to them.
    /// </summary>
    [Fact]
    public async Task GetMyPaymentReturnsOwnedPayment()
    {
        var userId = Guid.NewGuid();
        var customer = CreateCustomer(userId);
        var order = CreateOrder(customer.Id);
        var payment = CreatePayment(order);
        var paymentStore = new FakePaymentStore(order, payment);

        var service = CreateService(
            userId,
            customer,
            paymentStore);

        var result = await service.GetMyPaymentAsync(
            payment.Id,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(payment.Id, result.Id);
        Assert.Equal(order.Id, result.OrderId);
        Assert.Equal(payment.PaymentNumber, result.PaymentNumber);
        Assert.Equal("Cash", result.Method);
        Assert.Equal("Pending", result.Status);
    }

    /// <summary>
    /// Tests that payment retrieval returns null when the payment
    /// belongs to another customer.
    /// </summary>
    [Fact]
    public async Task GetMyPaymentWhenPaymentBelongsToAnotherCustomerReturnsNull()
    {
        var userId = Guid.NewGuid();
        var customer = CreateCustomer(userId);
        var anotherCustomer = CreateCustomer(Guid.NewGuid());
        var anotherCustomersOrder = CreateOrder(anotherCustomer.Id);
        var payment = CreatePayment(anotherCustomersOrder);
        var paymentStore = new FakePaymentStore(
            anotherCustomersOrder,
            payment);

        var service = CreateService(
            userId,
            customer,
            paymentStore);

        var result = await service.GetMyPaymentAsync(
            payment.Id,
            CancellationToken.None);

        Assert.Null(result);
    }

    /// <summary>
    /// Tests that payment history is returned for an order owned
    /// by the authenticated customer.
    /// </summary>
    [Fact]
    public async Task GetMyOrderPaymentsReturnsOrderPaymentHistory()
    {
        var userId = Guid.NewGuid();
        var customer = CreateCustomer(userId);
        var order = CreateOrder(customer.Id);
        var firstPayment = CreatePayment(order, "PAY-001");
        var secondPayment = CreatePayment(order, "PAY-002");
        var paymentStore = new FakePaymentStore(
            order,
            firstPayment,
            secondPayment);

        var service = CreateService(
            userId,
            customer,
            paymentStore);

        var result = await service.GetMyOrderPaymentsAsync(
            order.Id,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, item => item.Id == firstPayment.Id);
        Assert.Contains(result, item => item.Id == secondPayment.Id);
    }

    /// <summary>
    /// Tests that payment history is not returned for an order
    /// belonging to another customer.
    /// </summary>
    [Fact]
    public async Task GetMyOrderPaymentsForAnotherCustomersOrderReturnsNull()
    {
        var userId = Guid.NewGuid();
        var customer = CreateCustomer(userId);
        var anotherCustomer = CreateCustomer(Guid.NewGuid());
        var anotherCustomersOrder = CreateOrder(anotherCustomer.Id);
        var paymentStore = new FakePaymentStore(anotherCustomersOrder);

        var service = CreateService(
            userId,
            customer,
            paymentStore);

        var result = await service.GetMyOrderPaymentsAsync(
            anotherCustomersOrder.Id,
            CancellationToken.None);

        Assert.Null(result);
    }

    private static PaymentService CreateService(
        Guid userId,
        Customer customer,
        FakePaymentStore paymentStore,
        FakeUnitOfWork? unitOfWork = null)
    {
        return new PaymentService(
            new FakeCurrentUser(userId),
            new FakeCustomerStore(customer),
            paymentStore,
            unitOfWork ?? new FakeUnitOfWork());
    }

    private static Customer CreateCustomer(Guid userId)
    {
        return new Customer(userId, "Test Customer");
    }

    private static Order CreateOrder(Guid customerId)
    {
        return new Order(
            $"ORD-{Guid.NewGuid():N}",
            customerId,
            FulfillmentType.Pickup,
            new Money(250m, "INR"),
            new Money(0m, "INR"),
            null,
            null,
            "Test pickup");
    }

    private static Payment CreatePayment(
        Order order,
        string? paymentNumber = null)
    {
        return new Payment(
            order.Id,
            paymentNumber ?? $"PAY-{Guid.NewGuid():N}",
            PaymentMethod.Cash,
            order.TotalAmount);
    }

    private sealed class FakeCurrentUser : ICurrentUser
    {
        public FakeCurrentUser(Guid? userId)
        {
            UserId = userId;
        }

        public Guid? UserId { get; }

        public string? Role => UserRole.Customer.ToString();

        public bool IsAuthenticated => UserId.HasValue;
    }

    private sealed class FakeCustomerStore : ICustomerStore
    {
        private readonly Customer? customer;

        public FakeCustomerStore(Customer? customer = null)
        {
            this.customer = customer;
        }

        public Task<Customer?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                customer?.UserId == userId ? customer : null);
        }

        public void Add(Customer customer)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class FakePaymentStore : IPaymentStore
    {
        private readonly List<Order> orders;
        private readonly List<Payment> payments;

        public FakePaymentStore(
            Order? order = null,
            params Payment[] payments)
        {
            orders = order is null ? [] : [order];
            this.payments = payments.ToList();
        }

        public Payment? AddedPayment { get; private set; }

        public Task<Order?> GetCustomerOrderAsync(
            Guid customerId,
            Guid orderId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                orders.SingleOrDefault(
                    item => item.Id == orderId &&
                            item.CustomerId == customerId));
        }

        public Task<Payment?> GetCustomerPaymentAsync(
            Guid customerId,
            Guid paymentId,
            CancellationToken cancellationToken)
        {
            var payment = payments.SingleOrDefault(
                item => item.Id == paymentId);

            if (payment is null)
            {
                return Task.FromResult<Payment?>(null);
            }

            var order = orders.SingleOrDefault(
                item => item.Id == payment.OrderId &&
                        item.CustomerId == customerId);

            return Task.FromResult(order is null ? null : payment);
        }

        public Task<IReadOnlyCollection<Payment>> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken)
        {
            IReadOnlyCollection<Payment> result = payments
                .Where(payment => payment.OrderId == orderId)
                .ToArray();

            return Task.FromResult(result);
        }

        public void Add(Payment payment)
        {
            ArgumentNullException.ThrowIfNull(payment);

            AddedPayment = payment;
            payments.Add(payment);
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return Task.FromResult(1);
        }
    }
}
