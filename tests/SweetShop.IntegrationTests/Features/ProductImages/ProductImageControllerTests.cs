using System.Net;
using System.Net.Http.Json;
using SweetShop.Api.Common.Models;
using SweetShop.Application.Features.ProductImages.Responses;
using SweetShop.Domain.Enums;
using SweetShop.IntegrationTests.Infrastructure;
using Xunit;

namespace SweetShop.IntegrationTests.Features.ProductImages;

public sealed class ProductImageControllerTests
: IClassFixture<SweetShopApiFactory>
{
private readonly SweetShopApiFactory factory;


public ProductImageControllerTests(
    SweetShopApiFactory factory)
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
public async Task GetAdminImagesAsAdminReturnsOk()
{
    using var client = CreateAdminClient();

    var response = await client.GetAsync(
        $"/api/v1/admin/products/{TestProductImageService.ProductId}/images");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var content =
        await response.Content.ReadFromJsonAsync<
            ApiResponse<
                IReadOnlyCollection<ProductImageResponse>>>();

    Assert.NotNull(content);
    Assert.NotNull(content.Data);
    Assert.Equal(2, content.Data.Count);
    Assert.Contains(
        content.Data,
        image => image.Id == TestProductImageService.ImageId);
}

[Fact]
public async Task GetAdminImagesAsCustomerReturnsForbidden()
{
    using var client = CreateCustomerClient();

    var response = await client.GetAsync(
        $"/api/v1/admin/products/{TestProductImageService.ProductId}/images");

    Assert.Equal(
        HttpStatusCode.Forbidden,
        response.StatusCode);
}

[Fact]
public async Task GetAdminImageAsAdminReturnsOk()
{
    using var client = CreateAdminClient();

    var response = await client.GetAsync(
        $"/api/v1/admin/products/{TestProductImageService.ProductId}/images/{TestProductImageService.ImageId}");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var content =
        await response.Content.ReadFromJsonAsync<
            ApiResponse<ProductImageResponse>>();

    Assert.NotNull(content);
    Assert.NotNull(content.Data);
    Assert.Equal(
        TestProductImageService.ImageId,
        content.Data.Id);

    Assert.True(content.Data.IsPrimary);
}

[Fact]
public async Task GetMissingAdminImageReturnsNotFound()
{
    using var client = CreateAdminClient();

    var response = await client.GetAsync(
        $"/api/v1/admin/products/{TestProductImageService.ProductId}/images/{Guid.NewGuid()}");

    Assert.Equal(
        HttpStatusCode.NotFound,
        response.StatusCode);
}

[Fact]
public async Task CreateImageAsAdminReturnsCreated()
{
    using var client = CreateAdminClient();

    var request = new
    {
        url = "https://example.com/gulab-jamun.jpg",
        altText = "Gulab Jamun",
        displayOrder = 3,
        isPrimary = false
    };

    var response = await client.PostAsJsonAsync(
        $"/api/v1/admin/products/{TestProductImageService.ProductId}/images",
        request);

    Assert.Equal(
        HttpStatusCode.Created,
        response.StatusCode);
}

[Fact]
public async Task CreateImageAsCustomerReturnsForbidden()
{
    using var client = CreateCustomerClient();

    var request = new
    {
        url = "https://example.com/gulab-jamun.jpg",
        altText = "Gulab Jamun",
        displayOrder = 3,
        isPrimary = false
    };

    var response = await client.PostAsJsonAsync(
        $"/api/v1/admin/products/{TestProductImageService.ProductId}/images",
        request);

    Assert.Equal(
        HttpStatusCode.Forbidden,
        response.StatusCode);
}

[Fact]
public async Task UpdateImageAsAdminReturnsOk()
{
    using var client = CreateAdminClient();

    var request = new
    {
        url = "https://example.com/kaju-katli-updated.jpg",
        altText = "Updated Kaju Katli",
        displayOrder = 2
    };

    var response = await client.PutAsJsonAsync(
        $"/api/v1/admin/products/{TestProductImageService.ProductId}/images/{TestProductImageService.ImageId}",
        request);

    Assert.Equal(
        HttpStatusCode.OK,
        response.StatusCode);
}

[Fact]
public async Task UpdateMissingImageReturnsNotFound()
{
    using var client = CreateAdminClient();

    var request = new
    {
        url = "https://example.com/missing.jpg",
        altText = "Missing",
        displayOrder = 1
    };

    var response = await client.PutAsJsonAsync(
        $"/api/v1/admin/products/{TestProductImageService.ProductId}/images/{Guid.NewGuid()}",
        request);

    Assert.Equal(
        HttpStatusCode.NotFound,
        response.StatusCode);
}

[Fact]
public async Task DeleteImageAsAdminReturnsNoContent()
{
    using var client = CreateAdminClient();

    var response = await client.DeleteAsync(
        $"/api/v1/admin/products/{TestProductImageService.ProductId}/images/{TestProductImageService.ImageId}");

    Assert.Equal(
        HttpStatusCode.NoContent,
        response.StatusCode);
}

[Fact]
public async Task DeleteMissingImageReturnsNotFound()
{
    using var client = CreateAdminClient();

    var response = await client.DeleteAsync(
        $"/api/v1/admin/products/{TestProductImageService.ProductId}/images/{Guid.NewGuid()}");

    Assert.Equal(
        HttpStatusCode.NotFound,
        response.StatusCode);
}

[Fact]
public async Task SetPrimaryImageAsAdminReturnsNoContent()
{
    using var client = CreateAdminClient();

    var response = await client.PutAsync(
        $"/api/v1/admin/products/{TestProductImageService.ProductId}/images/{TestProductImageService.SecondaryImageId}/primary",
        content: null);

    Assert.Equal(
        HttpStatusCode.NoContent,
        response.StatusCode);
}

[Fact]
public async Task SetPrimaryMissingImageReturnsNotFound()
{
    using var client = CreateAdminClient();

    var response = await client.PutAsync(
        $"/api/v1/admin/products/{TestProductImageService.ProductId}/images/{Guid.NewGuid()}/primary",
        content: null);

    Assert.Equal(
        HttpStatusCode.NotFound,
        response.StatusCode);
}

[Fact]
public async Task GetCustomerImagesReturnsOk()
{
    using var client = CreateCustomerClient();

    var response = await client.GetAsync(
        $"/api/v1/products/{TestProductImageService.ProductId}/images");

    Assert.Equal(
        HttpStatusCode.OK,
        response.StatusCode);

    var content =
        await response.Content.ReadFromJsonAsync<
            ApiResponse<
                IReadOnlyCollection<ProductImageResponse>>>();

    Assert.NotNull(content);
    Assert.NotNull(content.Data);
    Assert.Equal(2, content.Data.Count);
}

[Fact]
public async Task GetCustomerImagesWithoutAuthenticationReturnsUnauthorized()
{
    using var client = factory.CreateClient();

    var response = await client.GetAsync(
        $"/api/v1/products/{TestProductImageService.ProductId}/images");

    Assert.Equal(
        HttpStatusCode.Unauthorized,
        response.StatusCode);
}

}
