using SweetShop.Application.Features.Categories;
using SweetShop.Application.Features.Products.Requests;
using SweetShop.Application.Features.Products.Responses;
using SweetShop.Application.Interfaces;
using SweetShop.Domain.Common;
using SweetShop.Domain.Entities;
using SweetShop.Domain.ValueObjects;

namespace SweetShop.Application.Features.Products;

/// <summary>
/// Provides application operations for managing products and variants.
/// </summary>
public sealed class ProductService : IProductService
{
    private readonly IProductStore productStore;
    private readonly IProductVariantStore productVariantStore;
    private readonly ICategoryStore categoryStore;
    private readonly IShopContext shopContext;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productStore"></param>
    /// <param name="productVariantStore"></param>
    /// <param name="categoryStore"></param>
    /// <param name="shopContext"></param>
    /// <param name="unitOfWork"></param>
    public ProductService(
        IProductStore productStore,
        IProductVariantStore productVariantStore,
        ICategoryStore categoryStore,
        IShopContext shopContext,
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(productStore);
        ArgumentNullException.ThrowIfNull(productVariantStore);
        ArgumentNullException.ThrowIfNull(categoryStore);
        ArgumentNullException.ThrowIfNull(shopContext);
        ArgumentNullException.ThrowIfNull(unitOfWork);

        this.productStore = productStore;
        this.productVariantStore = productVariantStore;
        this.categoryStore = categoryStore;
        this.shopContext = shopContext;
        this.unitOfWork = unitOfWork;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IReadOnlyCollection<ProductResponse>> GetProductsAsync(
        CancellationToken cancellationToken)
    {
        var products = await productStore.GetByShopIdAsync(
            shopContext.ShopId,
            cancellationToken);

        return products.Select(Map).ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IReadOnlyCollection<ProductResponse>> GetActiveProductsAsync(
        CancellationToken cancellationToken)
    {
        var products = await productStore.GetActiveByShopIdAsync(
            shopContext.ShopId,
            cancellationToken);

        return products.Select(Map).ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductResponse?> GetProductByIdAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var product = await productStore.GetByIdAsync(
            shopContext.ShopId,
            productId,
            cancellationToken);

        return product is null ? null : Map(product);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<ProductResponse> CreateProductAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var category = await categoryStore.GetByIdAsync(
            shopContext.ShopId,
            request.CategoryId,
            cancellationToken);

        if (category is null)
        {
            throw new InvalidOperationException(
                "The specified category does not exist.");
        }

        var existingProduct = await productStore.GetByNameAsync(
            shopContext.ShopId,
            request.CategoryId,
            request.Name,
            cancellationToken);

        if (existingProduct is not null)
        {
            throw new InvalidOperationException(
                "A product with the same name already exists in this category.");
        }

        var product = new Product(
            shopContext.ShopId,
            request.CategoryId,
            request.Name,
            request.Description,
            request.DisplayOrder);

        productStore.Add(product);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(product);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<ProductResponse?> UpdateProductAsync(
        Guid productId,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var product = await productStore.GetByIdAsync(
            shopContext.ShopId,
            productId,
            cancellationToken);

        if (product is null)
        {
            return null;
        }

        var category = await categoryStore.GetByIdAsync(
            shopContext.ShopId,
            request.CategoryId,
            cancellationToken);

        if (category is null)
        {
            throw new InvalidOperationException(
                "The specified category does not exist.");
        }

        var existingProduct = await productStore.GetByNameAsync(
            shopContext.ShopId,
            request.CategoryId,
            request.Name,
            cancellationToken);

        if (existingProduct is not null &&
            existingProduct.Id != product.Id)
        {
            throw new InvalidOperationException(
                "A product with the same name already exists in this category.");
        }

        if (product.CategoryId != request.CategoryId)
        {
            product.ChangeCategory(request.CategoryId);
        }

        product.UpdateInformation(
            request.Name,
            request.Description,
            request.DisplayOrder);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(product);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> DeactivateProductAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var product = await productStore.GetByIdAsync(
            shopContext.ShopId,
            productId,
            cancellationToken);

        if (product is null)
        {
            return false;
        }

        product.Deactivate();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="activeOnly"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IReadOnlyCollection<ProductVariantResponse>> GetVariantsAsync(
        Guid productId,
        bool activeOnly,
        CancellationToken cancellationToken)
    {
        await EnsureProductExistsAsync(productId, cancellationToken);

        var variants = activeOnly
            ? await productVariantStore.GetActiveByProductIdAsync(
                productId,
                cancellationToken)
            : await productVariantStore.GetByProductIdAsync(
                productId,
                cancellationToken);

        return variants.Select(Map).ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="variantId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductVariantResponse?> GetVariantByIdAsync(
        Guid productId,
        Guid variantId,
        CancellationToken cancellationToken)
    {
        await EnsureProductExistsAsync(productId, cancellationToken);

        var variant = await productVariantStore.GetByIdAsync(
            productId,
            variantId,
            cancellationToken);

        return variant is null ? null : Map(variant);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<ProductVariantResponse> CreateVariantAsync(
        Guid productId,
        CreateProductVariantRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        await EnsureProductExistsAsync(productId, cancellationToken);

        var existingVariant = await productVariantStore.GetByNameAsync(
            productId,
            request.Name,
            cancellationToken);

        if (existingVariant is not null)
        {
            throw new InvalidOperationException(
                "A product variant with the same name already exists.");
        }

        var quantity = new ProductQuantity(
            request.Quantity,
            request.Unit);

        var price = new Money(
            request.PriceAmount,
            DomainConstants.CurrencyInr);

        var variant = new ProductVariant(
            productId,
            request.Name,
            quantity,
            price,
            request.DisplayOrder);

        productVariantStore.Add(variant);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(variant);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="variantId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<ProductVariantResponse?> UpdateVariantAsync(
        Guid productId,
        Guid variantId,
        UpdateProductVariantRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        await EnsureProductExistsAsync(productId, cancellationToken);

        var variant = await productVariantStore.GetByIdAsync(
            productId,
            variantId,
            cancellationToken);

        if (variant is null)
        {
            return null;
        }

        var existingVariant = await productVariantStore.GetByNameAsync(
            productId,
            request.Name,
            cancellationToken);

        if (existingVariant is not null &&
            existingVariant.Id != variant.Id)
        {
            throw new InvalidOperationException(
                "A product variant with the same name already exists.");
        }

        var price = new Money(
            request.PriceAmount,
            DomainConstants.CurrencyInr);

        variant.UpdateInformation(
            request.Name,
            price,
            request.DisplayOrder);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(variant);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="variantId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> DeactivateVariantAsync(
        Guid productId,
        Guid variantId,
        CancellationToken cancellationToken)
    {
        await EnsureProductExistsAsync(productId, cancellationToken);

        var variant = await productVariantStore.GetByIdAsync(
            productId,
            variantId,
            cancellationToken);

        if (variant is null)
        {
            return false;
        }

        variant.Deactivate();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task EnsureProductExistsAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var product = await productStore.GetByIdAsync(
            shopContext.ShopId,
            productId,
            cancellationToken);

        if (product is null)
        {
            throw new InvalidOperationException(
                "The specified product does not exist.");
        }
    }

    private static ProductResponse Map(Product product)
    {
        return new ProductResponse(
            product.Id,
            product.ShopId,
            product.CategoryId,
            product.Name,
            product.Description,
            product.DisplayOrder,
            product.Status,
            product.CreatedAt,
            product.UpdatedAt);
    }

    private static ProductVariantResponse Map(ProductVariant variant)
    {
        return new ProductVariantResponse(
            variant.Id,
            variant.ProductId,
            variant.Name,
            variant.Quantity.Value,
            variant.Quantity.Unit,
            variant.Price.Amount,
            variant.Price.Currency,
            variant.DisplayOrder,
            variant.Status,
            variant.CreatedAt,
            variant.UpdatedAt);
    }

/// <summary>
/// 
/// </summary>
/// <param name="productId"></param>
/// <param name="cancellationToken"></param>
/// <param name="variantId"></param>
/// <returns></returns>
/// <exception cref="NotImplementedException"></exception>
   public async Task<bool> ActivateVariantAsync(
    Guid productId,
    Guid variantId,
    CancellationToken cancellationToken)
{
    var product = await productStore.GetByIdAsync(
        shopContext.ShopId,
        productId,
        cancellationToken);

    if (product is null)
    {
        return false;
    }

    var variant = await productVariantStore.GetByIdAsync(
        productId,
        variantId,
        cancellationToken);

    if (variant is null)
    {
        return false;
    }

    variant.Activate();

    await unitOfWork.SaveChangesAsync(cancellationToken);

    return true;
}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> ActivateProductAsync(
    Guid productId,
    CancellationToken cancellationToken)
    {
        var product = await productStore.GetByIdAsync(
            shopContext.ShopId,
            productId,
            cancellationToken);

        if (product is null)
        {
            return false;
        }

        product.Activate();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

}