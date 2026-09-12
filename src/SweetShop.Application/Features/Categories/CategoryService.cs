using SweetShop.Application.Features.Categories.Requests;
using SweetShop.Application.Features.Categories.Responses;
using SweetShop.Application.Interfaces;
using SweetShop.Domain.Entities;

namespace SweetShop.Application.Features.Categories;

/// <summary>
/// Provides application operations for managing product categories.
/// </summary>
public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryStore categoryStore;
    private readonly IShopContext shopContext;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryService"/> class.
    /// </summary>
    /// <param name="categoryStore">The category persistence store.</param>
    /// <param name="shopContext">The current shop context.</param>
    /// <param name="unitOfWork">The unit of work used to persist changes.</param>
    public CategoryService(
        ICategoryStore categoryStore,
        IShopContext shopContext,
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(categoryStore);
        ArgumentNullException.ThrowIfNull(shopContext);
        ArgumentNullException.ThrowIfNull(unitOfWork);

        this.categoryStore = categoryStore;
        this.shopContext = shopContext;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CategoryResponse>> GetCategoriesAsync(
        CancellationToken cancellationToken)
    {
        var categories = await categoryStore.GetByShopIdAsync(
            shopContext.ShopId,
            cancellationToken);

        return categories
            .Select(Map)
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CategoryResponse>> GetActiveCategoriesAsync(
        CancellationToken cancellationToken)
    {
        var categories = await categoryStore.GetActiveByShopIdAsync(
            shopContext.ShopId,
            cancellationToken);

        return categories
            .Select(Map)
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<CategoryResponse?> GetCategoryByIdAsync(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var category = await categoryStore.GetByIdAsync(
            shopContext.ShopId,
            categoryId,
            cancellationToken);

        return category is null
            ? null
            : Map(category);
    }

    /// <inheritdoc />
    public async Task<CategoryResponse> CreateCategoryAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existingCategory = await categoryStore.GetByNameAsync(
            shopContext.ShopId,
            request.Name,
            cancellationToken);

        if (existingCategory is not null)
        {
            throw new InvalidOperationException(
                "A category with the same name already exists.");
        }

        var category = new Category(
            shopContext.ShopId,
            request.Name,
            request.Description,
            request.ImageUrl,
            request.DisplayOrder);

        categoryStore.Add(category);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(category);
    }

    /// <inheritdoc />
    public async Task<CategoryResponse?> UpdateCategoryAsync(
        Guid categoryId,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var category = await categoryStore.GetByIdAsync(
            shopContext.ShopId,
            categoryId,
            cancellationToken);

        if (category is null)
        {
            return null;
        }

        var existingCategory = await categoryStore.GetByNameAsync(
            shopContext.ShopId,
            request.Name,
            cancellationToken);

        if (existingCategory is not null &&
            existingCategory.Id != category.Id)
        {
            throw new InvalidOperationException(
                "A category with the same name already exists.");
        }

        category.UpdateInformation(
            request.Name,
            request.Description,
            request.ImageUrl,
            request.DisplayOrder);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(category);
    }

    /// <inheritdoc />
    public async Task<bool> DeactivateCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var category = await categoryStore.GetByIdAsync(
            shopContext.ShopId,
            categoryId,
            cancellationToken);

        if (category is null)
        {
            return false;
        }

        category.Deactivate();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static CategoryResponse Map(Category category)
    {
        return new CategoryResponse(
            category.Id,
            category.ShopId,
            category.Name,
            category.Description,
            category.ImageUrl,
            category.DisplayOrder,
            category.Status,
            category.CreatedAt,
            category.UpdatedAt);
    }
}