using SweetShop.Application.Features.ProductImages;
using SweetShop.Application.Features.ProductImages.Requests;
using SweetShop.Application.Features.ProductImages.Responses;

namespace SweetShop.IntegrationTests.Infrastructure;

/// <summary>
/// Provides a test implementation of the product image service.
/// </summary>
public sealed class TestProductImageService : IProductImageService
{
public static readonly Guid ProductId =
Guid.Parse("33333333-3333-3333-3333-333333333333");

public static readonly Guid ImageId =
    Guid.Parse("77777777-7777-7777-7777-777777777777");

public static readonly Guid SecondaryImageId =
    Guid.Parse("88888888-8888-8888-8888-888888888888");

public Task<IReadOnlyCollection<ProductImageResponse>> GetImagesAsync(
    Guid productId,
    bool activeProductOnly,
    CancellationToken cancellationToken)
{
    if (productId != ProductId)
    {
        return Task.FromResult<
            IReadOnlyCollection<ProductImageResponse>>(
            Array.Empty<ProductImageResponse>());
    }

    return Task.FromResult<
        IReadOnlyCollection<ProductImageResponse>>(
        new[]
        {
            CreateImage(
                ImageId,
                "https://example.com/kaju-katli.jpg",
                "Kaju Katli",
                1,
                true),

            CreateImage(
                SecondaryImageId,
                "https://example.com/kaju-katli-box.jpg",
                "Kaju Katli Box",
                2,
                false)
        });
}

public Task<ProductImageResponse?> GetImageByIdAsync(
    Guid productId,
    Guid imageId,
    CancellationToken cancellationToken)
{
    if (productId != ProductId)
    {
        return Task.FromResult<ProductImageResponse?>(null);
    }

    if (imageId == ImageId)
    {
        return Task.FromResult<ProductImageResponse?>(
            CreateImage(
                ImageId,
                "https://example.com/kaju-katli.jpg",
                "Kaju Katli",
                1,
                true));
    }

    if (imageId == SecondaryImageId)
    {
        return Task.FromResult<ProductImageResponse?>(
            CreateImage(
                SecondaryImageId,
                "https://example.com/kaju-katli-box.jpg",
                "Kaju Katli Box",
                2,
                false));
    }

    return Task.FromResult<ProductImageResponse?>(null);
}

public Task<ProductImageResponse> CreateImageAsync(
    Guid productId,
    CreateProductImageRequest request,
    CancellationToken cancellationToken)
{
    if (productId != ProductId)
    {
        throw new InvalidOperationException(
            "The specified product does not exist.");
    }

    return Task.FromResult(
        CreateImage(
            ImageId,
            request.Url,
            request.AltText,
            request.DisplayOrder,
            request.IsPrimary));
}

public Task<ProductImageResponse?> UpdateImageAsync(
    Guid productId,
    Guid imageId,
    UpdateProductImageRequest request,
    CancellationToken cancellationToken)
{
    if (productId != ProductId ||
        (imageId != ImageId &&
         imageId != SecondaryImageId))
    {
        return Task.FromResult<ProductImageResponse?>(null);
    }

    return Task.FromResult<ProductImageResponse?>(
        CreateImage(
            imageId,
            request.Url,
            request.AltText,
            request.DisplayOrder,
            imageId == ImageId));
}

public Task<bool> DeleteImageAsync(
    Guid productId,
    Guid imageId,
    CancellationToken cancellationToken)
{
    return Task.FromResult(
        productId == ProductId &&
        (imageId == ImageId ||
         imageId == SecondaryImageId));
}

public Task<bool> SetPrimaryImageAsync(
    Guid productId,
    Guid imageId,
    CancellationToken cancellationToken)
{
    return Task.FromResult(
        productId == ProductId &&
        (imageId == ImageId ||
         imageId == SecondaryImageId));
}

private static ProductImageResponse CreateImage(
    Guid id,
    string url,
    string? altText,
    int displayOrder,
    bool isPrimary)
{
    var now = DateTime.UtcNow;

    return new ProductImageResponse(
        id,
        ProductId,
        url,
        altText,
        displayOrder,
        isPrimary,
        now,
        now);
}


}
