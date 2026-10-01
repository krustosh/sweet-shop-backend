using System.Net;
using System.Net.Http.Json;
using SweetShop.Api.Common.Models;
using SweetShop.Application.Features.Orders.Responses;
using SweetShop.Domain.Enums;
using SweetShop.IntegrationTests.Infrastructure;
using Xunit;

namespace SweetShop.IntegrationTests.Features.Orders;

/// <summary>
/// Provides integration tests for the customer order APIs.
/// </summary>
public sealed class OrderControllerTests
    : IClassFixture<SweetShopApiFactory>
{
    private static readonly Guid MissingOrderId =
        Guid.Parse("99999999-9999-9999-9999-999999999999");

    private readonly SweetShopApiFactory factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderControllerTests"/> class.
    /// </summary>
    /// <param name="factory">The API test factory.</param>
    public OrderControllerTests(SweetShopApiFactory factory)
    {
        this.factory = factory;
    }

    /// <summary>
    /// Creates an authenticated customer HTTP client.
    /// </summary>
    /// <returns>An authenticated customer HTTP client.</returns>
    private HttpClient CreateCustomerClient()
    {
        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-Test-Role",
            nameof(UserRole.Customer));

        client.DefaultRequestHeaders.Add(
            "X-Test-User-Id",
            SweetShopApiFactory.TestCustomerUserId.ToString());

        return client;
    }

    /// <summary>
    /// Verifies that an unauthenticated request cannot access customer orders.
    /// </summary>
    [Fact]
    public async Task GetOrdersWithoutAuthenticationReturnsUnauthorized()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/v1/customers/me/orders");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    /// <summary>
    /// Verifies that an authenticated customer can retrieve their orders.
    /// </summary>
    [Fact]
    public async Task GetOrdersReturnsCustomerOrders()
    {
        using var client = CreateCustomerClient();

        var response = await client.GetAsync(
            "/api/v1/customers/me/orders");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var content =
            await response.Content.ReadFromJsonAsync<
                ApiResponse<IReadOnlyCollection<OrderResponse>>>();

        Assert.NotNull(content);
        Assert.NotNull(content.Data);
    }

    /// <summary>
    /// Verifies that an unknown order returns not found.
    /// </summary>
    [Fact]
    public async Task GetMissingOrderReturnsNotFound()
    {
        using var client = CreateCustomerClient();

        var response = await client.GetAsync(
            $"/api/v1/customers/me/orders/{MissingOrderId}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    /// <summary>
    /// Verifies that an empty cart cannot be converted into an order.
    /// </summary>
    [Fact]
    public async Task CreateOrderWithEmptyCartReturnsBadRequest()
    {
        using var client = CreateCustomerClient();

        await client.DeleteAsync(
            "/api/v1/customers/me/cart");

        var response = await client.PostAsJsonAsync(
            "/api/v1/customers/me/orders",
            new
            {
                fulfillmentType = FulfillmentType.Pickup,
                deliveryAddressId = (Guid?)null,
                customerNote = "Test order"
            });

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    /// <summary>
    /// Verifies that pickup orders do not accept a delivery address.
    /// </summary>
    [Fact]
    public async Task CreatePickupOrderWithDeliveryAddressReturnsBadRequest()
    {
        using var client = CreateCustomerClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/customers/me/orders",
            new
            {
                fulfillmentType = FulfillmentType.Pickup,
                deliveryAddressId = Guid.NewGuid(),
                customerNote = "Test order"
            });

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    /// <summary>
    /// Verifies that delivery orders require a delivery address.
    /// </summary>
    [Fact]
    public async Task CreateDeliveryOrderWithoutAddressReturnsBadRequest()
    {
        using var client = CreateCustomerClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/customers/me/orders",
            new
            {
                fulfillmentType = FulfillmentType.Delivery,
                deliveryAddressId = (Guid?)null,
                customerNote = "Test order"
            });

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    /// <summary>
    /// Verifies that an invalid cancellation request is rejected.
    /// </summary>
    [Fact]
    public async Task CancelOrderWithoutReasonReturnsBadRequest()
    {
        using var client = CreateCustomerClient();

        var response = await client.PostAsJsonAsync(
            $"/api/v1/customers/me/orders/{MissingOrderId}/cancel",
            new
            {
                reason = string.Empty
            });

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    /// <summary>
    /// Verifies that cancelling an unknown order returns not found.
    /// </summary>
    [Fact]
    public async Task CancelMissingOrderReturnsNotFound()
    {
        using var client = CreateCustomerClient();

        var response = await client.PostAsJsonAsync(
            $"/api/v1/customers/me/orders/{MissingOrderId}/cancel",
            new
            {
                reason = "Customer requested cancellation."
            });

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }
}