using SweetShop.Application.Features.ProductImages.Requests;
using SweetShop.Application.Features.ProductImages.Responses;
using SweetShop.Application.Features.Products;
using SweetShop.Application.Interfaces;
using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;

namespace SweetShop.Application.Features.ProductImages;

/// <summary>
/// Provides application operations for managing product images.
/// </summary>
public sealed class ProductImageService : IProductImageService
{
    private readonly IProductImageStore productImageStore;
    private readonly IProductStore productStore;
    private readonly IShopContext shopContext;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productImageStore"></param>
    /// <param name="productStore"></param>
    /// <param name="shopContext"></param>
    /// <param name="unitOfWork"></param>
    public ProductImageService(
        IProductImageStore productImageStore,
        IProductStore productStore,
        IShopContext shopContext,
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(productImageStore);
        ArgumentNullException.ThrowIfNull(productStore);
        ArgumentNullException.ThrowIfNull(shopContext);
        ArgumentNullException.ThrowIfNull(unitOfWork);

        this.productImageStore = productImageStore;
        this.productStore = productStore;
        this.shopContext = shopContext;
        this.unitOfWork = unitOfWork;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="activeProductOnly"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IReadOnlyCollection<ProductImageResponse>> GetImagesAsync(
        Guid productId,
        bool activeProductOnly,
        CancellationToken cancellationToken)
    {
        var product = await productStore.GetByIdAsync(
            shopContext.ShopId,
            productId,
            cancellationToken);

        if (product is null ||
            (activeProductOnly && product.Status != ProductStatus.Active))
        {
            return Array.Empty<ProductImageResponse>();
        }

        var images = await productImageStore.GetByProductIdAsync(
            productId,
            cancellationToken);

        return images.Select(Map).ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="imageId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductImageResponse?> GetImageByIdAsync(
        Guid productId,
        Guid imageId,
        CancellationToken cancellationToken)
    {
        await EnsureProductExistsAsync(
            productId,
            cancellationToken);

        var image = await productImageStore.GetByIdAsync(
            productId,
            imageId,
            cancellationToken);

        return image is null ? null : Map(image);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductImageResponse> CreateImageAsync(
        Guid productId,
        CreateProductImageRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        await EnsureProductExistsAsync(
            productId,
            cancellationToken);

        var image = new ProductImage(
            productId,
            request.Url,
            request.DisplayOrder,
            request.IsPrimary);

        image.UpdateInformation(
            request.Url,
            request.AltText,
            request.DisplayOrder);

        if (request.IsPrimary)
        {
            await RemoveExistingPrimaryAsync(
                productId,
                cancellationToken);

            image.MarkAsPrimary();
        }

        productImageStore.Add(image);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(image);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="imageId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductImageResponse?> UpdateImageAsync(
        Guid productId,
        Guid imageId,
        UpdateProductImageRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        await EnsureProductExistsAsync(
            productId,
            cancellationToken);

        var image = await productImageStore.GetByIdAsync(
            productId,
            imageId,
            cancellationToken);

        if (image is null)
        {
            return null;
        }

        image.UpdateInformation(
            request.Url,
            request.AltText,
            request.DisplayOrder);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(image);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="imageId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> DeleteImageAsync(
        Guid productId,
        Guid imageId,
        CancellationToken cancellationToken)
    {
        await EnsureProductExistsAsync(
            productId,
            cancellationToken);

        var image = await productImageStore.GetByIdAsync(
            productId,
            imageId,
            cancellationToken);

        if (image is null)
        {
            return false;
        }

        productImageStore.Remove(image);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="imageId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> SetPrimaryImageAsync(
        Guid productId,
        Guid imageId,
        CancellationToken cancellationToken)
    {
        await EnsureProductExistsAsync(
            productId,
            cancellationToken);

        var image = await productImageStore.GetByIdAsync(
            productId,
            imageId,
            cancellationToken);

        if (image is null)
        {
            return false;
        }

        await RemoveExistingPrimaryAsync(
            productId,
            cancellationToken);

        image.MarkAsPrimary();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task RemoveExistingPrimaryAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var existingPrimary =
            await productImageStore.GetPrimaryByProductIdAsync(
                productId,
                cancellationToken);

        existingPrimary?.RemovePrimaryDesignation();
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

    private static ProductImageResponse Map(
        ProductImage image)
    {
        return new ProductImageResponse(
            image.Id,
            image.ProductId,
            image.Url,
            image.AltText,
            image.DisplayOrder,
            image.IsPrimary,
            image.CreatedAt,
            image.UpdatedAt);
    }
}