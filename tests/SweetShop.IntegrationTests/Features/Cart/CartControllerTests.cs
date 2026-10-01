using System.Net;
using System.Net.Http.Json;
using SweetShop.Api.Common.Models;
using SweetShop.Application.Features.Cart.Responses;
using SweetShop.Domain.Enums;
using SweetShop.IntegrationTests.Infrastructure;
using Xunit;

namespace SweetShop.IntegrationTests.Features.Cart;

/// <summary>
/// Provides integration tests for the customer cart APIs.
/// </summary>
public sealed class CartControllerTests : IClassFixture<SweetShopApiFactory>
{
    private static readonly Guid VariantId =
        SweetShopApiFactory.TestVariantId;

    private static readonly Guid MissingVariantId =
        Guid.Parse("99999999-9999-9999-9999-999999999999");

    private readonly SweetShopApiFactory factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartControllerTests"/> class.
    /// </summary>
    /// <param name="factory">The API test factory.</param>
    public CartControllerTests(SweetShopApiFactory factory)
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
    /// Verifies that an unauthenticated request cannot access the cart.
    /// </summary>
    [Fact]
    public async Task GetCartWithoutAuthenticationReturnsUnauthorized()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/v1/customers/me/cart");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Verifies that a customer can retrieve an empty cart.
    /// </summary>
    [Fact]
    public async Task GetCartReturnsCart()
    {
        using var client = CreateCustomerClient();
        await client.DeleteAsync("/api/v1/customers/me/cart");

        var response = await client.GetAsync(
            "/api/v1/customers/me/cart");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<
            ApiResponse<CartResponse>>();

        Assert.NotNull(content);
        Assert.NotNull(content.Data);
        Assert.Equal(SweetShopApiFactory.TestCustomerId, content.Data.CustomerId);
        Assert.Empty(content.Data.Items);
        Assert.Equal(0m, content.Data.Subtotal);
    }

    /// <summary>
    /// Verifies that a customer can add an active product variant to the cart.
    /// </summary>
    [Fact]
    public async Task AddItemReturnsUpdatedCart()
    {
        using var client = CreateCustomerClient();
        await client.DeleteAsync("/api/v1/customers/me/cart");

        var response = await client.PostAsJsonAsync(
            "/api/v1/customers/me/cart/items",
            new
            {
                productVariantId = VariantId,
                quantity = 2m
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<
            ApiResponse<CartResponse>>();

        Assert.NotNull(content?.Data);
        var item = Assert.Single(content.Data.Items);
        Assert.Equal(VariantId, item.ProductVariantId);
        Assert.Equal(2m, item.Quantity);
        Assert.Equal(450m, item.UnitPrice);
        Assert.Equal(900m, item.LineTotal);
        Assert.Equal(900m, content.Data.Subtotal);
    }

    /// <summary>
    /// Verifies that adding an existing variant increases its quantity rather than creating a duplicate item.
    /// </summary>
    [Fact]
    public async Task AddExistingItemIncreasesQuantity()
    {
        using var client = CreateCustomerClient();
        await client.DeleteAsync("/api/v1/customers/me/cart");

        await client.PostAsJsonAsync(
            "/api/v1/customers/me/cart/items",
            new { productVariantId = VariantId, quantity = 2m });

        var response = await client.PostAsJsonAsync(
            "/api/v1/customers/me/cart/items",
            new { productVariantId = VariantId, quantity = 3m });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<
            ApiResponse<CartResponse>>();

        Assert.NotNull(content?.Data);
        var item = Assert.Single(content.Data.Items);
        Assert.Equal(5m, item.Quantity);
        Assert.Equal(2250m, item.LineTotal);
    }

    /// <summary>
    /// Verifies that a customer can change an existing cart-item quantity.
    /// </summary>
    [Fact]
    public async Task UpdateItemChangesQuantity()
    {
        using var client = CreateCustomerClient();
        await client.DeleteAsync("/api/v1/customers/me/cart");

        var addResponse = await client.PostAsJsonAsync(
            "/api/v1/customers/me/cart/items",
            new { productVariantId = VariantId, quantity = 2m });
        var addContent = await addResponse.Content.ReadFromJsonAsync<
            ApiResponse<CartResponse>>();
        var itemId = Assert.Single(addContent!.Data!.Items).Id;

        var response = await client.PutAsJsonAsync(
            $"/api/v1/customers/me/cart/items/{itemId}",
            new { quantity = 4m });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<
            ApiResponse<CartResponse>>();
        Assert.NotNull(content?.Data);
        Assert.Equal(4m, Assert.Single(content.Data.Items).Quantity);
        Assert.Equal(1800m, content.Data.Subtotal);
    }

    /// <summary>
    /// Verifies that a customer can remove an item from the cart.
    /// </summary>
    [Fact]
    public async Task RemoveItemRemovesCartItem()
    {
        using var client = CreateCustomerClient();
        await client.DeleteAsync("/api/v1/customers/me/cart");

        var addResponse = await client.PostAsJsonAsync(
            "/api/v1/customers/me/cart/items",
            new { productVariantId = VariantId, quantity = 1m });
        var addContent = await addResponse.Content.ReadFromJsonAsync<
            ApiResponse<CartResponse>>();
        var itemId = Assert.Single(addContent!.Data!.Items).Id;

        var response = await client.DeleteAsync(
            $"/api/v1/customers/me/cart/items/{itemId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var cartResponse = await client.GetAsync(
            "/api/v1/customers/me/cart");
        var cartContent = await cartResponse.Content.ReadFromJsonAsync<
            ApiResponse<CartResponse>>();

        Assert.Empty(cartContent!.Data!.Items);
    }

    /// <summary>
    /// Verifies that clearing the cart removes all cart items.
    /// </summary>
    [Fact]
    public async Task ClearCartRemovesAllItems()
    {
        using var client = CreateCustomerClient();
        await client.DeleteAsync("/api/v1/customers/me/cart");

        await client.PostAsJsonAsync(
            "/api/v1/customers/me/cart/items",
            new { productVariantId = VariantId, quantity = 2m });

        var response = await client.DeleteAsync(
            "/api/v1/customers/me/cart");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var cartResponse = await client.GetAsync(
            "/api/v1/customers/me/cart");
        var cartContent = await cartResponse.Content.ReadFromJsonAsync<
            ApiResponse<CartResponse>>();

        Assert.Empty(cartContent!.Data!.Items);
    }

    /// <summary>
    /// Verifies that an unavailable product variant cannot be added to the cart.
    /// </summary>
    [Fact]
    public async Task AddMissingVariantReturnsBadRequest()
    {
        using var client = CreateCustomerClient();
        await client.DeleteAsync("/api/v1/customers/me/cart");

        var response = await client.PostAsJsonAsync(
            "/api/v1/customers/me/cart/items",
            new
            {
                productVariantId = MissingVariantId,
                quantity = 1m
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Verifies that a zero quantity is rejected by validation.
    /// </summary>
    [Fact]
    public async Task AddZeroQuantityReturnsBadRequest()
    {
        using var client = CreateCustomerClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/customers/me/cart/items",
            new
            {
                productVariantId = VariantId,
                quantity = 0m
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Verifies that a customer cannot update another customer's cart item.
    /// </summary>
    [Fact]
    public async Task UpdateMissingCartItemReturnsNotFound()
    {
        using var client = CreateCustomerClient();

        var response = await client.PutAsJsonAsync(
            $"/api/v1/customers/me/cart/items/{Guid.NewGuid()}",
            new { quantity = 2m });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
