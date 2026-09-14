using System.Net;
using System.Net.Http.Json;
using SweetShop.Api.Common.Models;
using SweetShop.Application.Features.Products.Responses;
using SweetShop.Domain.Enums;
using SweetShop.IntegrationTests.Infrastructure;
using Xunit;

namespace SweetShop.IntegrationTests.Features.Products;

public sealed class ProductControllerTests
: IClassFixture<SweetShopApiFactory>
{
private static readonly Guid ProductId =
Guid.Parse("33333333-3333-3333-3333-333333333333");

private static readonly Guid InactiveProductId =
    Guid.Parse("44444444-4444-4444-4444-444444444444");

private static readonly Guid VariantId =
    Guid.Parse("55555555-5555-5555-5555-555555555555");

private static readonly Guid InactiveVariantId =
    Guid.Parse("66666666-6666-6666-6666-666666666666");

private static readonly Guid CategoryId =
    Guid.Parse("22222222-2222-2222-2222-222222222222");

private readonly SweetShopApiFactory factory;

public ProductControllerTests(SweetShopApiFactory factory)
{
    this.factory = factory;
}

private HttpClient CreateClient(string role)
{
    var client = factory.CreateClient();

    client.DefaultRequestHeaders.Add(
        "X-Test-Role",
        role);

    return client;
}

private HttpClient CreateAdminClient()
{
    return CreateClient(nameof(UserRole.Admin));
}

private HttpClient CreateCustomerClient()
{
    return CreateClient(nameof(UserRole.Customer));
}

[Fact]
public async Task GetAdminProductsAsAdminReturnsOk()
{
    using var client = CreateAdminClient();

    var response = await client.GetAsync(
        "/api/v1/admin/products");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var content =
        await response.Content.ReadFromJsonAsync<
            ApiResponse<IReadOnlyCollection<ProductResponse>>>();

    Assert.NotNull(content);
    Assert.NotNull(content.Data);
    Assert.Contains(
        content.Data,
        product => product.Id == ProductId);
}

[Fact]
public async Task GetAdminProductsAsCustomerReturnsForbidden()
{
    using var client = CreateCustomerClient();

    var response = await client.GetAsync(
        "/api/v1/admin/products");

    Assert.Equal(
        HttpStatusCode.Forbidden,
        response.StatusCode);
}

[Fact]
public async Task GetProductAsAdminReturnsOk()
{
    using var client = CreateAdminClient();

    var response = await client.GetAsync(
        $"/api/v1/admin/products/{ProductId}");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var content =
        await response.Content.ReadFromJsonAsync<
            ApiResponse<ProductResponse>>();

    Assert.NotNull(content);
    Assert.NotNull(content.Data);
    Assert.Equal(ProductId, content.Data.Id);
    Assert.Equal("Kaju Katli", content.Data.Name);
}

[Fact]
public async Task GetMissingProductAsAdminReturnsNotFound()
{
    using var client = CreateAdminClient();

    var response = await client.GetAsync(
        $"/api/v1/admin/products/{Guid.NewGuid()}");

    Assert.Equal(
        HttpStatusCode.NotFound,
        response.StatusCode);
}

[Fact]
public async Task CreateProductAsAdminReturnsCreated()
{
    using var client = CreateAdminClient();

    var request = new
    {
        categoryId = CategoryId,
        name = "Gulab Jamun",
        description = "Fresh gulab jamun",
        displayOrder = 2
    };

    var response = await client.PostAsJsonAsync(
        "/api/v1/admin/products",
        request);

    Assert.Equal(
        HttpStatusCode.Created,
        response.StatusCode);
}

[Fact]
public async Task CreateProductAsCustomerReturnsForbidden()
{
    using var client = CreateCustomerClient();

    var request = new
    {
        categoryId = CategoryId,
        name = "Gulab Jamun",
        description = "Fresh gulab jamun",
        displayOrder = 2
    };

    var response = await client.PostAsJsonAsync(
        "/api/v1/admin/products",
        request);

    Assert.Equal(
        HttpStatusCode.Forbidden,
        response.StatusCode);
}

[Fact]
public async Task UpdateProductAsAdminReturnsOk()
{
    using var client = CreateAdminClient();

    var request = new
    {
        categoryId = CategoryId,
        name = "Kaju Katli Premium",
        description = "Premium kaju katli",
        displayOrder = 1
    };

    var response = await client.PutAsJsonAsync(
        $"/api/v1/admin/products/{ProductId}",
        request);

    Assert.Equal(
        HttpStatusCode.OK,
        response.StatusCode);
}

[Fact]
public async Task UpdateMissingProductAsAdminReturnsNotFound()
{
    using var client = CreateAdminClient();

    var request = new
    {
        categoryId = CategoryId,
        name = "Missing Product",
        description = "Missing product",
        displayOrder = 1
    };

    var response = await client.PutAsJsonAsync(
        $"/api/v1/admin/products/{Guid.NewGuid()}",
        request);

    Assert.Equal(
        HttpStatusCode.NotFound,
        response.StatusCode);
}

[Fact]
public async Task DeactivateProductAsAdminReturnsNoContent()
{
    using var client = CreateAdminClient();

    var response = await client.DeleteAsync(
        $"/api/v1/admin/products/{ProductId}");

    Assert.Equal(
        HttpStatusCode.NoContent,
        response.StatusCode);
}

[Fact]
public async Task DeactivateMissingProductAsAdminReturnsNotFound()
{
    using var client = CreateAdminClient();

    var response = await client.DeleteAsync(
        $"/api/v1/admin/products/{Guid.NewGuid()}");

    Assert.Equal(
        HttpStatusCode.NotFound,
        response.StatusCode);
}

[Fact]
public async Task ActivateProductAsAdminReturnsNoContent()
{
    using var client = CreateAdminClient();

    var response = await client.PutAsync(
        $"/api/v1/admin/products/{InactiveProductId}/activate",
        content: null);

    Assert.Equal(
        HttpStatusCode.NoContent,
        response.StatusCode);
}

[Fact]
public async Task ActivateMissingProductAsAdminReturnsNotFound()
{
    using var client = CreateAdminClient();

    var response = await client.PutAsync(
        $"/api/v1/admin/products/{Guid.NewGuid()}/activate",
        content: null);

    Assert.Equal(
        HttpStatusCode.NotFound,
        response.StatusCode);
}

[Fact]
public async Task GetActiveProductsAsCustomerReturnsOk()
{
    using var client = CreateCustomerClient();

    var response = await client.GetAsync(
        "/api/v1/products");

    Assert.Equal(
        HttpStatusCode.OK,
        response.StatusCode);

    var json =
        await response.Content.ReadAsStringAsync();

    Assert.Contains("Kaju Katli", json);
    Assert.DoesNotContain("Old Sweet", json);
}

[Fact]
public async Task GetActiveProductsWithoutAuthenticationReturnsUnauthorized()
{
    using var client = factory.CreateClient();

    var response = await client.GetAsync(
        "/api/v1/products");

    Assert.Equal(
        HttpStatusCode.Unauthorized,
        response.StatusCode);
}

[Fact]
public async Task GetAdminVariantsAsAdminReturnsOk()
{
    using var client = CreateAdminClient();

    var response = await client.GetAsync(
        $"/api/v1/admin/products/{ProductId}/variants");

    Assert.Equal(
        HttpStatusCode.OK,
        response.StatusCode);

    var content =
        await response.Content.ReadFromJsonAsync<
            ApiResponse<IReadOnlyCollection<ProductVariantResponse>>>();

    Assert.NotNull(content);
    Assert.NotNull(content.Data);
    Assert.Contains(
        content.Data,
        variant => variant.Id == VariantId);
}

[Fact]
public async Task GetAdminVariantsAsCustomerReturnsForbidden()
{
    using var client = CreateCustomerClient();

    var response = await client.GetAsync(
        $"/api/v1/admin/products/{ProductId}/variants");

    Assert.Equal(
        HttpStatusCode.Forbidden,
        response.StatusCode);
}

[Fact]
public async Task GetVariantAsAdminReturnsOk()
{
    using var client = CreateAdminClient();

    var response = await client.GetAsync(
        $"/api/v1/admin/products/{ProductId}/variants/{VariantId}");

    Assert.Equal(
        HttpStatusCode.OK,
        response.StatusCode);

    var content =
        await response.Content.ReadFromJsonAsync<
            ApiResponse<ProductVariantResponse>>();

    Assert.NotNull(content);
    Assert.NotNull(content.Data);
    Assert.Equal(VariantId, content.Data.Id);
    Assert.Equal("500g Box", content.Data.Name);
}

[Fact]
public async Task GetMissingVariantAsAdminReturnsNotFound()
{
    using var client = CreateAdminClient();

    var response = await client.GetAsync(
        $"/api/v1/admin/products/{ProductId}/variants/{Guid.NewGuid()}");

    Assert.Equal(
        HttpStatusCode.NotFound,
        response.StatusCode);
}

[Fact]
public async Task CreateVariantAsAdminReturnsCreated()
{
    using var client = CreateAdminClient();

    var request = new
    {
        name = "1kg Box",
        quantity = 1.0m,
        unit = ProductUnit.Kilogram,
        priceAmount = 900.00m,
        displayOrder = 2
    };

    var response = await client.PostAsJsonAsync(
        $"/api/v1/admin/products/{ProductId}/variants",
        request);

    Assert.Equal(
        HttpStatusCode.Created,
        response.StatusCode);
}

[Fact]
public async Task CreateVariantAsCustomerReturnsForbidden()
{
    using var client = CreateCustomerClient();

    var request = new
    {
        name = "1kg Box",
        quantity = 1.0m,
        unit = ProductUnit.Kilogram,
        priceAmount = 900.00m,
        displayOrder = 2
    };

    var response = await client.PostAsJsonAsync(
        $"/api/v1/admin/products/{ProductId}/variants",
        request);

    Assert.Equal(
        HttpStatusCode.Forbidden,
        response.StatusCode);
}

[Fact]
public async Task UpdateVariantAsAdminReturnsOk()
{
    using var client = CreateAdminClient();

    var request = new
    {
        name = "500g Premium Box",
        priceAmount = 500.00m,
        displayOrder = 1
    };

    var response = await client.PutAsJsonAsync(
        $"/api/v1/admin/products/{ProductId}/variants/{VariantId}",
        request);

    Assert.Equal(
        HttpStatusCode.OK,
        response.StatusCode);
}

[Fact]
public async Task UpdateMissingVariantAsAdminReturnsNotFound()
{
    using var client = CreateAdminClient();

    var request = new
    {
        name = "Missing Variant",
        priceAmount = 500.00m,
        displayOrder = 1
    };

    var response = await client.PutAsJsonAsync(
        $"/api/v1/admin/products/{ProductId}/variants/{Guid.NewGuid()}",
        request);

    Assert.Equal(
        HttpStatusCode.NotFound,
        response.StatusCode);
}

[Fact]
public async Task DeactivateVariantAsAdminReturnsNoContent()
{
    using var client = CreateAdminClient();

    var response = await client.DeleteAsync(
        $"/api/v1/admin/products/{ProductId}/variants/{VariantId}");

    Assert.Equal(
        HttpStatusCode.NoContent,
        response.StatusCode);
}

[Fact]
public async Task DeactivateMissingVariantAsAdminReturnsNotFound()
{
    using var client = CreateAdminClient();

    var response = await client.DeleteAsync(
        $"/api/v1/admin/products/{ProductId}/variants/{Guid.NewGuid()}");

    Assert.Equal(
        HttpStatusCode.NotFound,
        response.StatusCode);
}

[Fact]
public async Task ActivateVariantAsAdminReturnsNoContent()
{
    using var client = CreateAdminClient();

    var response = await client.PutAsync(
        $"/api/v1/admin/products/{ProductId}/variants/{InactiveVariantId}/activate",
        content: null);

    Assert.Equal(
        HttpStatusCode.NoContent,
        response.StatusCode);
}

[Fact]
public async Task ActivateMissingVariantAsAdminReturnsNotFound()
{
    using var client = CreateAdminClient();

    var response = await client.PutAsync(
        $"/api/v1/admin/products/{ProductId}/variants/{Guid.NewGuid()}/activate",
        content: null);

    Assert.Equal(
        HttpStatusCode.NotFound,
        response.StatusCode);
}

[Fact]
public async Task GetActiveVariantsAsCustomerReturnsOk()
{
    using var client = CreateCustomerClient();

    var response = await client.GetAsync(
        $"/api/v1/products/{ProductId}/variants");

    Assert.Equal(
        HttpStatusCode.OK,
        response.StatusCode);

    var json =
        await response.Content.ReadAsStringAsync();

    Assert.Contains("500g Box", json);
    Assert.DoesNotContain("500 Gram", json);
}

[Fact]
public async Task GetActiveVariantsWithoutAuthenticationReturnsUnauthorized()
{
    using var client = factory.CreateClient();

    var response = await client.GetAsync(
        $"/api/v1/products/{ProductId}/variants");

    Assert.Equal(
        HttpStatusCode.Unauthorized,
        response.StatusCode);
}

}
