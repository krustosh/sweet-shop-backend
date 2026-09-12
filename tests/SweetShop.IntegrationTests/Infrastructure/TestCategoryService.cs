using SweetShop.Application.Features.Categories;
using SweetShop.Application.Features.Categories.Requests;
using SweetShop.Application.Features.Categories.Responses;
using SweetShop.Domain.Enums;

namespace SweetShop.IntegrationTests.Infrastructure;

internal sealed class TestCategoryService : ICategoryService
{
    private static readonly Guid ActiveCategoryId =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static readonly Guid InactiveCategoryId =
        Guid.Parse("33333333-3333-3333-3333-333333333333");

    private static readonly DateTime CreatedAt =
        new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static readonly DateTime UpdatedAt =
        new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public Task<IReadOnlyCollection<CategoryResponse>> GetCategoriesAsync(
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<CategoryResponse>>(
            new[]
            {
                CreateCategoryResponse(
                    ActiveCategoryId,
                    "Traditional Sweets",
                    CategoryStatus.Active),

                CreateCategoryResponse(
                    InactiveCategoryId,
                    "Seasonal Sweets",
                    CategoryStatus.Inactive)
            });
    }

    public Task<IReadOnlyCollection<CategoryResponse>> GetActiveCategoriesAsync(
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<CategoryResponse>>(
            new[]
            {
                CreateCategoryResponse(
                    ActiveCategoryId,
                    "Traditional Sweets",
                    CategoryStatus.Active)
            });
    }

    public Task<CategoryResponse?> GetCategoryByIdAsync(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        if (categoryId == ActiveCategoryId)
        {
            return Task.FromResult<CategoryResponse?>(
                CreateCategoryResponse(
                    ActiveCategoryId,
                    "Traditional Sweets",
                    CategoryStatus.Active));
        }

        if (categoryId == InactiveCategoryId)
        {
            return Task.FromResult<CategoryResponse?>(
                CreateCategoryResponse(
                    InactiveCategoryId,
                    "Seasonal Sweets",
                    CategoryStatus.Inactive));
        }

        return Task.FromResult<CategoryResponse?>(null);
    }

    public Task<CategoryResponse> CreateCategoryAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(
            CreateCategoryResponse(
                Guid.Parse("44444444-4444-4444-4444-444444444444"),
                request.Name,
                CategoryStatus.Active));
    }

    public Task<CategoryResponse?> UpdateCategoryAsync(
        Guid categoryId,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        if (categoryId != ActiveCategoryId)
        {
            return Task.FromResult<CategoryResponse?>(null);
        }

        return Task.FromResult<CategoryResponse?>(
            CreateCategoryResponse(
                categoryId,
                request.Name,
                CategoryStatus.Active));
    }

    public Task<bool> DeactivateCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(categoryId == ActiveCategoryId);
    }

    private static CategoryResponse CreateCategoryResponse(
        Guid id,
        string name,
        CategoryStatus status)
    {
        return new CategoryResponse(
            id,
            SweetShopApiFactory.TestShopId,
            name,
            "Integration test category",
            null,
            1,
            status,
            CreatedAt,
            UpdatedAt);
    }
}