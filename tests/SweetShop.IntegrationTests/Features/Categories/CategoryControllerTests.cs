using System.Net;
using System.Net.Http.Json;
using SweetShop.IntegrationTests.Infrastructure;

namespace SweetShop.IntegrationTests.Features.Categories;

public sealed class CategoryControllerTests
    : IClassFixture<SweetShopApiFactory>
{
    private readonly HttpClient client;

    public CategoryControllerTests(SweetShopApiFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCustomerCategoriesWithoutAuthenticationReturnsUnauthorized()
    {
        var response = await client.GetAsync(
            "/api/v1/categories");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task GetAdminCategoriesWithoutAuthenticationReturnsUnauthorized()
    {
        var response = await client.GetAsync(
            "/api/v1/admin/categories");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task GetAdminCategoriesAsCustomerReturnsForbidden()
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/v1/admin/categories");

        request.Headers.Add("X-Test-Role", "Customer");

        var response = await client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    [Fact]
    public async Task GetAdminCategoriesAsAdminReturnsOk()
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/v1/admin/categories");

        request.Headers.Add("X-Test-Role", "Admin");

        var response = await client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var body =
            await response.Content.ReadFromJsonAsync<
                ApiResponseTestModel<List<CategoryTestModel>>>();

        Assert.NotNull(body);
        Assert.NotNull(body.Data);
        Assert.Equal(2, body.Data.Count);
    }

    [Fact]
    public async Task GetCustomerCategoriesAsCustomerReturnsOnlyActiveCategories()
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/v1/categories");

        request.Headers.Add("X-Test-Role", "Customer");

        var response = await client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var body =
            await response.Content.ReadFromJsonAsync<
                ApiResponseTestModel<List<CategoryTestModel>>>();

        Assert.NotNull(body);
        Assert.NotNull(body.Data);
        Assert.Single(body.Data);

        Assert.Equal(
            "Traditional Sweets",
            body.Data[0].Name);
    }

    [Fact]
    public async Task GetCategoryAsAdminReturnsOk()
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/v1/admin/categories/22222222-2222-2222-2222-222222222222");

        request.Headers.Add("X-Test-Role", "Admin");

        var response = await client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var body =
            await response.Content.ReadFromJsonAsync<
                ApiResponseTestModel<CategoryTestModel>>();

        Assert.NotNull(body);
        Assert.NotNull(body.Data);
        Assert.Equal(
            "Traditional Sweets",
            body.Data.Name);
    }

    [Fact]
    public async Task GetMissingCategoryAsAdminReturnsNotFound()
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/v1/admin/categories/99999999-9999-9999-9999-999999999999");

        request.Headers.Add("X-Test-Role", "Admin");

        var response = await client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateCategoryAsAdminReturnsCreated()
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/v1/admin/categories")
        {
            Content = JsonContent.Create(new
            {
                name = "Dry Fruits",
                description = "Premium dry fruit sweets",
                imageUrl = (string?)null,
                displayOrder = 2
            })
        };

        request.Headers.Add("X-Test-Role", "Admin");

        var response = await client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var body =
            await response.Content.ReadFromJsonAsync<
                ApiResponseTestModel<CategoryTestModel>>();

        Assert.NotNull(body);
        Assert.NotNull(body.Data);
        Assert.Equal(
            "Dry Fruits",
            body.Data.Name);
    }

    [Fact]
    public async Task UpdateCategoryAsAdminReturnsOk()
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Put,
            "/api/v1/admin/categories/22222222-2222-2222-2222-222222222222")
        {
            Content = JsonContent.Create(new
            {
                name = "Premium Traditional Sweets",
                description = "Updated description",
                imageUrl = (string?)null,
                displayOrder = 1
            })
        };

        request.Headers.Add("X-Test-Role", "Admin");

        var response = await client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var body =
            await response.Content.ReadFromJsonAsync<
                ApiResponseTestModel<CategoryTestModel>>();

        Assert.NotNull(body);
        Assert.NotNull(body.Data);
        Assert.Equal(
            "Premium Traditional Sweets",
            body.Data.Name);
    }

    [Fact]
    public async Task UpdateMissingCategoryAsAdminReturnsNotFound()
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Put,
            "/api/v1/admin/categories/99999999-9999-9999-9999-999999999999")
        {
            Content = JsonContent.Create(new
            {
                name = "Updated Category",
                description = "Updated description",
                imageUrl = (string?)null,
                displayOrder = 1
            })
        };

        request.Headers.Add("X-Test-Role", "Admin");

        var response = await client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task DeactivateCategoryAsAdminReturnsNoContent()
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Delete,
            "/api/v1/admin/categories/22222222-2222-2222-2222-222222222222");

        request.Headers.Add("X-Test-Role", "Admin");

        var response = await client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);
    }

    [Fact]
    public async Task DeactivateMissingCategoryAsAdminReturnsNotFound()
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Delete,
            "/api/v1/admin/categories/99999999-9999-9999-9999-999999999999");

        request.Headers.Add("X-Test-Role", "Admin");

        var response = await client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    private sealed class ApiResponseTestModel<T>
    {
        public T? Data { get; init; }
    }

    private sealed class CategoryTestModel
    {
        public string Name { get; init; } = string.Empty;
    }
}