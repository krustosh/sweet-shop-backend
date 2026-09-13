using SweetShop.Application.Features.Categories;
using SweetShop.Application.Features.Categories.Requests;
using SweetShop.Application.Interfaces;
using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;

namespace SweetShop.UnitTests.Features.Categories;

/// <summary>
/// Represents a suite of unit tests for the <see cref="CategoryService"/> class.
/// </summary>
public sealed class CategoryServiceTests
{
    /// <summary>
    /// Tests that the GetCategoriesAsync method returns categories for the current shop.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetCategoriesReturnsCategoriesForCurrentShop()
    {
        var shopId = Guid.NewGuid();
        var category = CreateCategory(shopId, "Kaju Sweets");

        var shopContext = new FakeShopContext(shopId);
        var categoryStore = new FakeCategoryStore(category);
        var service = CreateService(categoryStore, shopContext);

        var result = await service.GetCategoriesAsync(
            CancellationToken.None);

        var returnedCategory = Assert.Single(result);

        Assert.Equal(category.Id, returnedCategory.Id);
        Assert.Equal(shopId, returnedCategory.ShopId);
        Assert.Equal("Kaju Sweets", returnedCategory.Name);
    }

    /// <summary>
    /// Tests that the GetActiveCategoriesAsync method returns only active categories for the current shop.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetActiveCategoriesReturnsOnlyActiveCategories()
    {
        var shopId = Guid.NewGuid();

        var activeCategory = CreateCategory(
            shopId,
            "Kaju Sweets");

        var inactiveCategory = CreateCategory(
            shopId,
            "Old Sweets");

        inactiveCategory.Deactivate();

        var shopContext = new FakeShopContext(shopId);
        var categoryStore = new FakeCategoryStore(
            activeCategory,
            inactiveCategory);

        var service = CreateService(
            categoryStore,
            shopContext);

        var result = await service.GetActiveCategoriesAsync(
            CancellationToken.None);

        var returnedCategory = Assert.Single(result);

        Assert.Equal(
            activeCategory.Id,
            returnedCategory.Id);

        Assert.Equal(
            CategoryStatus.Active,
            returnedCategory.Status);
    }

    /// <summary>
    /// Tests that the GetCategoryByIdAsync method returns the category for the current shop when it exists.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetCategoryByIdReturnsCategoryFromCurrentShop()
    {
        var shopId = Guid.NewGuid();
        var category = CreateCategory(
            shopId,
            "Laddoos");

        var shopContext = new FakeShopContext(shopId);
        var categoryStore = new FakeCategoryStore(category);

        var service = CreateService(
            categoryStore,
            shopContext);

        var result = await service.GetCategoryByIdAsync(
            category.Id,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(category.Id, result.Id);
        Assert.Equal("Laddoos", result.Name);
    }

    /// <summary>
    /// Tests that the GetCategoryByIdAsync method returns null when the category does not exist for the current shop.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetCategoryByIdWhenCategoryDoesNotExistReturnsNull()
    {
        var shopContext = new FakeShopContext(Guid.NewGuid());
        var categoryStore = new FakeCategoryStore();

        var service = CreateService(
            categoryStore,
            shopContext);

        var result = await service.GetCategoryByIdAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.Null(result);
    }

    /// <summary>
    /// Tests that the CreateCategoryAsync method creates a new category for the current shop.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateCategoryCreatesCategory()
    {
        var shopId = Guid.NewGuid();
        var shopContext = new FakeShopContext(shopId);
        var categoryStore = new FakeCategoryStore();
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            categoryStore,
            shopContext,
            unitOfWork);

        var request = new CreateCategoryRequest(
            "Bengali Sweets",
            "Traditional Bengali sweets",
            "https://example.com/bengali.jpg",
            2);

        var result = await service.CreateCategoryAsync(
            request,
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(shopId, result.ShopId);
        Assert.Equal("Bengali Sweets", result.Name);
        Assert.Equal(
            "Traditional Bengali sweets",
            result.Description);
        Assert.Equal(
            "https://example.com/bengali.jpg",
            result.ImageUrl);
        Assert.Equal(2, result.DisplayOrder);
        Assert.Equal(
            CategoryStatus.Active,
            result.Status);

        Assert.NotNull(categoryStore.AddedCategory);
        Assert.Equal(
            1,
            unitOfWork.SaveChangesCallCount);
    }

    /// <summary>
    /// Tests that the CreateCategoryAsync method throws an exception when a category with the same name already exists for the current shop.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateCategoryWhenNameAlreadyExistsThrows()
    {
        var shopId = Guid.NewGuid();

        var existingCategory = CreateCategory(
            shopId,
            "Kaju Sweets");

        var shopContext = new FakeShopContext(shopId);
        var categoryStore = new FakeCategoryStore(
            existingCategory);

        var service = CreateService(
            categoryStore,
            shopContext);

        var request = new CreateCategoryRequest(
            "Kaju Sweets",
            null,
            null,
            0);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateCategoryAsync(
                request,
                CancellationToken.None));

        Assert.Equal(
            "A category with the same name already exists.",
            exception.Message);
    }

    /// <summary>
    /// Tests that the CreateCategoryAsync method allows creating categories with the same name for different shops.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateCategoryAllowsSameNameForDifferentShop()
    {
        var existingShopId = Guid.NewGuid();
        var currentShopId = Guid.NewGuid();

        var existingCategory = CreateCategory(
            existingShopId,
            "Kaju Sweets");

        var shopContext = new FakeShopContext(currentShopId);
        var categoryStore = new FakeCategoryStore(
            existingCategory);
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            categoryStore,
            shopContext,
            unitOfWork);

        var request = new CreateCategoryRequest(
            "Kaju Sweets",
            null,
            null,
            0);

        var result = await service.CreateCategoryAsync(
            request,
            CancellationToken.None);

        Assert.Equal(currentShopId, result.ShopId);
        Assert.NotNull(categoryStore.AddedCategory);
        Assert.Equal(
            1,
            unitOfWork.SaveChangesCallCount);
    }

    /// <summary>
    ///  
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateCategoryUpdatesCategory()
    {
        var shopId = Guid.NewGuid();

        var category = CreateCategory(
            shopId,
            "Old Name");

        var shopContext = new FakeShopContext(shopId);
        var categoryStore = new FakeCategoryStore(category);
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            categoryStore,
            shopContext,
            unitOfWork);

        var request = new UpdateCategoryRequest(
            "Updated Name",
            "Updated description",
            "https://example.com/updated.jpg",
            5);

        var result = await service.UpdateCategoryAsync(
            category.Id,
            request,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(category.Id, result.Id);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal(
            "Updated description",
            result.Description);
        Assert.Equal(
            "https://example.com/updated.jpg",
            result.ImageUrl);
        Assert.Equal(5, result.DisplayOrder);
        Assert.Equal(
            1,
            unitOfWork.SaveChangesCallCount);
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateCategoryWhenCategoryDoesNotExistReturnsNull()
    {
        var shopContext = new FakeShopContext(Guid.NewGuid());
        var categoryStore = new FakeCategoryStore();

        var service = CreateService(
            categoryStore,
            shopContext);

        var request = new UpdateCategoryRequest(
            "Updated Name",
            null,
            null,
            0);

        var result = await service.UpdateCategoryAsync(
            Guid.NewGuid(),
            request,
            CancellationToken.None);

        Assert.Null(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateCategoryWhenNameAlreadyExistsThrows()
    {
        var shopId = Guid.NewGuid();

        var category = CreateCategory(
            shopId,
            "Kaju Sweets");

        var otherCategory = CreateCategory(
            shopId,
            "Laddoos");

        var shopContext = new FakeShopContext(shopId);
        var categoryStore = new FakeCategoryStore(
            category,
            otherCategory);

        var service = CreateService(
            categoryStore,
            shopContext);

        var request = new UpdateCategoryRequest(
            "Laddoos",
            null,
            null,
            0);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.UpdateCategoryAsync(
                category.Id,
                request,
                CancellationToken.None));

        Assert.Equal(
            "A category with the same name already exists.",
            exception.Message);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateCategoryAllowsKeepingSameName()
    {
        var shopId = Guid.NewGuid();

        var category = CreateCategory(
            shopId,
            "Kaju Sweets");

        var shopContext = new FakeShopContext(shopId);
        var categoryStore = new FakeCategoryStore(category);
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            categoryStore,
            shopContext,
            unitOfWork);

        var request = new UpdateCategoryRequest(
            "Kaju Sweets",
            "Updated description",
            null,
            3);

        var result = await service.UpdateCategoryAsync(
            category.Id,
            request,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Kaju Sweets", result.Name);
        Assert.Equal(
            "Updated description",
            result.Description);
        Assert.Equal(3, result.DisplayOrder);
        Assert.Equal(
            1,
            unitOfWork.SaveChangesCallCount);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeactivateCategoryDeactivatesCategory()
    {
        var shopId = Guid.NewGuid();

        var category = CreateCategory(
            shopId,
            "Kaju Sweets");

        var shopContext = new FakeShopContext(shopId);
        var categoryStore = new FakeCategoryStore(category);
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            categoryStore,
            shopContext,
            unitOfWork);

        var result = await service.DeactivateCategoryAsync(
            category.Id,
            CancellationToken.None);

        Assert.True(result);
        Assert.Equal(
            CategoryStatus.Inactive,
            category.Status);
        Assert.Equal(
            1,
            unitOfWork.SaveChangesCallCount);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeactivateCategoryWhenCategoryDoesNotExistReturnsFalse()
    {
        var shopContext = new FakeShopContext(Guid.NewGuid());
        var categoryStore = new FakeCategoryStore();
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            categoryStore,
            shopContext,
            unitOfWork);

        var result = await service.DeactivateCategoryAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.False(result);
        Assert.Equal(
            0,
            unitOfWork.SaveChangesCallCount);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetCategoriesUsesCurrentShop()
    {
        var shopId = Guid.NewGuid();
        var otherShopId = Guid.NewGuid();

        var currentShopCategory = CreateCategory(
            shopId,
            "Kaju Sweets");

        var otherShopCategory = CreateCategory(
            otherShopId,
            "Laddoos");

        var shopContext = new FakeShopContext(shopId);
        var categoryStore = new FakeCategoryStore(
            currentShopCategory,
            otherShopCategory);

        var service = CreateService(
            categoryStore,
            shopContext);

        var result = await service.GetCategoriesAsync(
            CancellationToken.None);

        var returnedCategory = Assert.Single(result);

        Assert.Equal(
            currentShopCategory.Id,
            returnedCategory.Id);

        Assert.Equal(
            shopId,
            returnedCategory.ShopId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetActiveCategoriesUsesCurrentShop()
    {
        var shopId = Guid.NewGuid();
        var otherShopId = Guid.NewGuid();

        var currentShopCategory = CreateCategory(
            shopId,
            "Kaju Sweets");

        var otherShopCategory = CreateCategory(
            otherShopId,
            "Laddoos");

        var shopContext = new FakeShopContext(shopId);
        var categoryStore = new FakeCategoryStore(
            currentShopCategory,
            otherShopCategory);

        var service = CreateService(
            categoryStore,
            shopContext);

        var result = await service.GetActiveCategoriesAsync(
            CancellationToken.None);

        var returnedCategory = Assert.Single(result);

        Assert.Equal(
            currentShopCategory.Id,
            returnedCategory.Id);

        Assert.Equal(
            shopId,
            returnedCategory.ShopId);
    }

    private static CategoryService CreateService(
        FakeCategoryStore categoryStore,
        FakeShopContext shopContext,
        FakeUnitOfWork? unitOfWork = null)
    {
        return new CategoryService(
            categoryStore,
            shopContext,
            unitOfWork ?? new FakeUnitOfWork());
    }

    private static Category CreateCategory(
        Guid shopId,
        string name)
    {
        return new Category(
            shopId,
            name,
            null,
            null,
            0);
    }

    private sealed class FakeShopContext : IShopContext
    {
        public FakeShopContext(Guid shopId)
        {
            ShopId = shopId;
        }

        public Guid ShopId { get; }
    }

    private sealed class FakeCategoryStore : ICategoryStore
    {
        private readonly IReadOnlyCollection<Category> categories;

        public FakeCategoryStore(
            params Category[] categories)
        {
            this.categories = categories;
        }

        public Category? AddedCategory { get; private set; }

        public Task<IReadOnlyCollection<Category>> GetByShopIdAsync(
            Guid shopId,
            CancellationToken cancellationToken)
        {
            IReadOnlyCollection<Category> result = categories
                .Where(category => category.ShopId == shopId)
                .OrderBy(category => category.DisplayOrder)
                .ThenBy(category => category.Name)
                .ToArray();

            return Task.FromResult(result);
        }

        public Task<IReadOnlyCollection<Category>> GetActiveByShopIdAsync(
            Guid shopId,
            CancellationToken cancellationToken)
        {
            IReadOnlyCollection<Category> result = categories
                .Where(category =>
                    category.ShopId == shopId &&
                    category.Status == CategoryStatus.Active)
                .OrderBy(category => category.DisplayOrder)
                .ThenBy(category => category.Name)
                .ToArray();

            return Task.FromResult(result);
        }

        public Task<Category?> GetByIdAsync(
            Guid shopId,
            Guid categoryId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                categories.SingleOrDefault(category =>
                    category.ShopId == shopId &&
                    category.Id == categoryId));
        }

        public Task<Category?> GetByNameAsync(
            Guid shopId,
            string name,
            CancellationToken cancellationToken)
        {
            var normalizedName = name.Trim();

            return Task.FromResult(
                categories.SingleOrDefault(category =>
                    category.ShopId == shopId &&
                    category.Name == normalizedName));
        }

        public void Add(Category category)
        {
            ArgumentNullException.ThrowIfNull(category);

            AddedCategory = category;
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