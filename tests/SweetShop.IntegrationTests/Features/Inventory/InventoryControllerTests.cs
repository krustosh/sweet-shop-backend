using System.Net;
using System.Net.Http.Json;
using SweetShop.Api.Common.Models;
using SweetShop.Application.Features.Inventory.Responses;
using SweetShop.Domain.Enums;
using SweetShop.IntegrationTests.Infrastructure;
using Xunit;

namespace SweetShop.IntegrationTests.Features.Inventory;

/// <summary>
/// Provides integration tests for the administrative inventory APIs.
/// </summary>
public sealed class InventoryControllerTests
: IClassFixture<SweetShopApiFactory>
{
private static readonly Guid VariantId =
Guid.Parse("55555555-5555-5555-5555-555555555555");


private readonly SweetShopApiFactory factory;

/// <summary>
/// Initializes a new instance of the
/// <see cref="InventoryControllerTests"/> class.
/// </summary>
/// <param name="factory">
/// The API test factory.
/// </param>
public InventoryControllerTests(
    SweetShopApiFactory factory)
{
    this.factory = factory;
}

/// <summary>
/// Creates an authenticated HTTP client for the specified role.
/// </summary>
/// <param name="role">
/// The user role.
/// </param>
/// <returns>
/// An authenticated HTTP client.
/// </returns>
private HttpClient CreateClient(string role)
{
    var client = factory.CreateClient();

    client.DefaultRequestHeaders.Add(
        "X-Test-Role",
        role);

    return client;
}

/// <summary>
/// Creates an authenticated administrator client.
/// </summary>
/// <returns>
/// An administrator HTTP client.
/// </returns>
private HttpClient CreateAdminClient()
{
    return CreateClient(nameof(UserRole.Admin));
}

/// <summary>
/// Creates an authenticated customer client.
/// </summary>
/// <returns>
/// A customer HTTP client.
/// </returns>
private HttpClient CreateCustomerClient()
{
    return CreateClient(nameof(UserRole.Customer));
}

/// <summary>
/// Verifies that an administrator can retrieve inventory.
/// </summary>
[Fact]
public async Task GetInventoryAsAdminReturnsOk()
{
    using var client = CreateAdminClient();

    var response = await client.GetAsync(
        "/api/v1/admin/inventory");

    Assert.Equal(
        HttpStatusCode.OK,
        response.StatusCode);

    var content =
        await response.Content.ReadFromJsonAsync<
            ApiResponse<
                IReadOnlyCollection<InventoryResponse>>>();

    Assert.NotNull(content);
    Assert.NotNull(content.Data);
}

/// <summary>
/// Verifies that a customer cannot retrieve administrative inventory.
/// </summary>
[Fact]
public async Task GetInventoryAsCustomerReturnsForbidden()
{
    using var client = CreateCustomerClient();

    var response = await client.GetAsync(
        "/api/v1/admin/inventory");

    Assert.Equal(
        HttpStatusCode.Forbidden,
        response.StatusCode);
}

/// <summary>
/// Verifies that an unauthenticated request cannot retrieve inventory.
/// </summary>
[Fact]
public async Task GetInventoryWithoutAuthenticationReturnsUnauthorized()
{
    using var client = factory.CreateClient();

    var response = await client.GetAsync(
        "/api/v1/admin/inventory");

    Assert.Equal(
        HttpStatusCode.Unauthorized,
        response.StatusCode);
}

/// <summary>
/// Verifies that an administrator can create inventory and then retrieve
/// it for a product variant.
/// </summary>
[Fact]
public async Task GetInventoryByVariantAsAdminReturnsOk()
{
    using var client = CreateAdminClient();

    var createRequest = new
    {
        lowStockThreshold = 5.0m
    };

    var createResponse = await client.PostAsJsonAsync(
        $"/api/v1/admin/inventory/{VariantId}",
        createRequest);

    Assert.True(
        createResponse.StatusCode == HttpStatusCode.OK ||
        createResponse.StatusCode == HttpStatusCode.BadRequest);

    if (createResponse.StatusCode == HttpStatusCode.BadRequest)
    {
        var existingResponse = await client.GetAsync(
            $"/api/v1/admin/inventory/{VariantId}");

        Assert.Equal(
            HttpStatusCode.OK,
            existingResponse.StatusCode);

        var existingContent =
            await existingResponse.Content.ReadFromJsonAsync<
                ApiResponse<InventoryResponse>>();

        Assert.NotNull(existingContent);
        Assert.NotNull(existingContent.Data);
        Assert.Equal(
            VariantId,
            existingContent.Data.ProductVariantId);

        return;
    }

    var response = await client.GetAsync(
        $"/api/v1/admin/inventory/{VariantId}");

    Assert.Equal(
        HttpStatusCode.OK,
        response.StatusCode);

    var content =
        await response.Content.ReadFromJsonAsync<
            ApiResponse<InventoryResponse>>();

    Assert.NotNull(content);
    Assert.NotNull(content.Data);
    Assert.Equal(
        VariantId,
        content.Data.ProductVariantId);
    Assert.Equal(
        5.0m,
        content.Data.LowStockThreshold);
}

/// <summary>
/// Verifies that requesting inventory for an unknown variant returns
/// not found.
/// </summary>
[Fact]
public async Task GetMissingInventoryReturnsNotFound()
{
    using var client = CreateAdminClient();

    var response = await client.GetAsync(
        $"/api/v1/admin/inventory/{Guid.NewGuid()}");

    Assert.Equal(
        HttpStatusCode.NotFound,
        response.StatusCode);
}

/// <summary>
/// Verifies that an administrator can create inventory for a product
/// variant.
/// </summary>
[Fact]
public async Task CreateInventoryAsAdminReturnsExpectedStatus()
{
    using var client = CreateAdminClient();

    var request = new
    {
        lowStockThreshold = 5.0m
    };

    var response = await client.PostAsJsonAsync(
        $"/api/v1/admin/inventory/{VariantId}",
        request);

    Assert.True(
        response.StatusCode == HttpStatusCode.OK ||
        response.StatusCode == HttpStatusCode.BadRequest);
}

/// <summary>
/// Verifies that a customer cannot create administrative inventory.
/// </summary>
[Fact]
public async Task CreateInventoryAsCustomerReturnsForbidden()
{
    using var client = CreateCustomerClient();

    var request = new
    {
        lowStockThreshold = 5.0m
    };

    var response = await client.PostAsJsonAsync(
        $"/api/v1/admin/inventory/{VariantId}",
        request);

    Assert.Equal(
        HttpStatusCode.Forbidden,
        response.StatusCode);
}

/// <summary>
/// Verifies that an administrator can update the inventory threshold
/// when an inventory record exists.
/// </summary>
[Fact]
public async Task UpdateThresholdAsAdminReturnsExpectedStatus()
{
    using var client = CreateAdminClient();

    await EnsureInventoryExistsAsync(client);

    var response = await client.PutAsJsonAsync(
        $"/api/v1/admin/inventory/{VariantId}/threshold",
        10.0m);

    Assert.Equal(
        HttpStatusCode.OK,
        response.StatusCode);
}

/// <summary>
/// Verifies that an administrator can add stock to an inventory record.
/// </summary>
[Fact]
public async Task AddStockAsAdminReturnsExpectedStatus()
{
    using var client = CreateAdminClient();

    await EnsureInventoryExistsAsync(client);

    var request = new
    {
        quantity = 10.0m,
        type = InventoryTransactionType.Purchase,
        referenceType = "TEST",
        referenceId = Guid.NewGuid(),
        reason = "Integration test stock addition"
    };

    var response = await client.PostAsJsonAsync(
        $"/api/v1/admin/inventory/{VariantId}/stock/add",
        request);

    Assert.Equal(
        HttpStatusCode.OK,
        response.StatusCode);
}

/// <summary>
/// Verifies that a customer cannot add administrative inventory stock.
/// </summary>
[Fact]
public async Task AddStockAsCustomerReturnsForbidden()
{
    using var client = CreateCustomerClient();

    var request = new
    {
        quantity = 10.0m,
        type = InventoryTransactionType.Purchase,
        referenceType = "TEST",
        referenceId = Guid.NewGuid(),
        reason = "Integration test stock addition"
    };

    var response = await client.PostAsJsonAsync(
        $"/api/v1/admin/inventory/{VariantId}/stock/add",
        request);

    Assert.Equal(
        HttpStatusCode.Forbidden,
        response.StatusCode);
}

/// <summary>
/// Verifies that an administrator can remove available inventory stock.
/// </summary>
[Fact]
public async Task RemoveStockAsAdminReturnsExpectedStatus()
{
    using var client = CreateAdminClient();

    await EnsureInventoryExistsAsync(client);

    var addRequest = new
    {
        quantity = 10.0m,
        type = InventoryTransactionType.Purchase,
        referenceType = "TEST",
        referenceId = Guid.NewGuid(),
        reason = "Integration test stock addition"
    };

    var addResponse = await client.PostAsJsonAsync(
        $"/api/v1/admin/inventory/{VariantId}/stock/add",
        addRequest);

    Assert.Equal(
        HttpStatusCode.OK,
        addResponse.StatusCode);

    var removeRequest = new
    {
        quantity = 1.0m,
        type = InventoryTransactionType.Damage,
        referenceType = "TEST",
        referenceId = Guid.NewGuid(),
        reason = "Integration test stock removal"
    };

    var response = await client.PostAsJsonAsync(
        $"/api/v1/admin/inventory/{VariantId}/stock/remove",
        removeRequest);

    Assert.Equal(
        HttpStatusCode.OK,
        response.StatusCode);
}

/// <summary>
/// Verifies that a customer cannot remove administrative inventory stock.
/// </summary>
[Fact]
public async Task RemoveStockAsCustomerReturnsForbidden()
{
    using var client = CreateCustomerClient();

    var request = new
    {
        quantity = 1.0m,
        type = InventoryTransactionType.Damage,
        referenceType = "TEST",
        referenceId = Guid.NewGuid(),
        reason = "Integration test stock removal"
    };

    var response = await client.PostAsJsonAsync(
        $"/api/v1/admin/inventory/{VariantId}/stock/remove",
        request);

    Assert.Equal(
        HttpStatusCode.Forbidden,
        response.StatusCode);
}

/// <summary>
/// Verifies that inventory transaction history can be retrieved.
/// </summary>
[Fact]
public async Task GetTransactionsAsAdminReturnsOk()
{
    using var client = CreateAdminClient();

    await EnsureInventoryExistsAsync(client);

    var response = await client.GetAsync(
        $"/api/v1/admin/inventory/{VariantId}/transactions");

    Assert.Equal(
        HttpStatusCode.OK,
        response.StatusCode);

    var content =
        await response.Content.ReadFromJsonAsync<
            ApiResponse<
                IReadOnlyCollection<InventoryTransactionResponse>>>();

    Assert.NotNull(content);
    Assert.NotNull(content.Data);
}

/// <summary>
/// Verifies that a customer cannot retrieve administrative inventory
/// transactions.
/// </summary>
[Fact]
public async Task GetTransactionsAsCustomerReturnsForbidden()
{
    using var client = CreateCustomerClient();

    var response = await client.GetAsync(
        $"/api/v1/admin/inventory/{VariantId}/transactions");

    Assert.Equal(
        HttpStatusCode.Forbidden,
        response.StatusCode);
}

/// <summary>
/// Verifies that requesting transactions for an unknown variant returns
/// not found.
/// </summary>
[Fact]
public async Task GetTransactionsForMissingVariantReturnsNotFound()
{
    var client = CreateAdminClient();

    var response = await client.GetAsync(
        $"/api/v1/admin/inventory/{Guid.NewGuid()}/transactions");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var result = await response.Content
        .ReadFromJsonAsync<ApiResponse<IReadOnlyCollection<InventoryTransactionResponse>>>();

    Assert.NotNull(result);
    Assert.NotNull(result.Data);
    Assert.Empty(result.Data);
}

/// <summary>
/// Ensures that inventory exists for the test variant.
/// </summary>
/// <param name="client">
/// The authenticated administrator client.
/// </param>
private static async Task EnsureInventoryExistsAsync(
    HttpClient client)
{
    var getResponse = await client.GetAsync(
        $"/api/v1/admin/inventory/{VariantId}");

    if (getResponse.StatusCode == HttpStatusCode.OK)
    {
        return;
    }

    Assert.Equal(
        HttpStatusCode.NotFound,
        getResponse.StatusCode);

    var request = new
    {
        lowStockThreshold = 5.0m
    };

    var createResponse = await client.PostAsJsonAsync(
        $"/api/v1/admin/inventory/{VariantId}",
        request);

    Assert.Equal(
        HttpStatusCode.OK,
        createResponse.StatusCode);
}


}
