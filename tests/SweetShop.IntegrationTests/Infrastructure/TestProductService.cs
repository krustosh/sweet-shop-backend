using SweetShop.Application.Features.Products;
using SweetShop.Application.Features.Products.Requests;
using SweetShop.Application.Features.Products.Responses;
using SweetShop.Domain.Enums;

namespace SweetShop.IntegrationTests.Infrastructure;

public sealed class TestProductService : IProductService
{
    private static readonly Guid ProductId =
        Guid.Parse("33333333-3333-3333-3333-333333333333");

    private static readonly Guid InactiveProductId =
        Guid.Parse("44444444-4444-4444-4444-444444444444");

    private static readonly Guid VariantId =
        Guid.Parse("55555555-5555-5555-5555-555555555555");

    private static readonly Guid InactiveVariantId =
        Guid.Parse("66666666-6666-6666-6666-666666666666");

    public Task<IReadOnlyCollection<ProductResponse>> GetProductsAsync(
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<ProductResponse>>(
            new[]
            {
                CreateProduct(
                    ProductId,
                    "Kaju Katli",
                    ProductStatus.Active),

                CreateProduct(
                    InactiveProductId,
                    "Old Sweet",
                    ProductStatus.Inactive)
            });
    }

    public Task<IReadOnlyCollection<ProductResponse>> GetActiveProductsAsync(
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<ProductResponse>>(
            new[]
            {
                CreateProduct(
                    ProductId,
                    "Kaju Katli",
                    ProductStatus.Active)
            });
    }

    public Task<ProductResponse?> GetProductByIdAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        if (productId == ProductId)
        {
            return Task.FromResult<ProductResponse?>(
                CreateProduct(
                    ProductId,
                    "Kaju Katli",
                    ProductStatus.Active));
        }

        return Task.FromResult<ProductResponse?>(null);
    }

    public Task<ProductResponse> CreateProductAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(
            CreateProduct(
                ProductId,
                request.Name,
                ProductStatus.Active));
    }

    public Task<ProductResponse?> UpdateProductAsync(
        Guid productId,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        if (productId != ProductId)
        {
            return Task.FromResult<ProductResponse?>(null);
        }

        return Task.FromResult<ProductResponse?>(
            CreateProduct(
                productId,
                request.Name,
                ProductStatus.Active));
    }

    public Task<bool> DeactivateProductAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(productId == ProductId);
    }

    public Task<IReadOnlyCollection<ProductVariantResponse>> GetVariantsAsync(
        Guid productId,
        bool activeOnly,
        CancellationToken cancellationToken)
    {
        if (productId != ProductId)
        {
            throw new InvalidOperationException(
                "The specified product does not exist.");
        }

        var variants = new[]
        {
            CreateVariant(
                VariantId,
                "500g Box",
                ProductVariantStatus.Active),

            CreateVariant(
                InactiveVariantId,
                "1kg Box",
                ProductVariantStatus.Inactive)
        };

        return Task.FromResult<IReadOnlyCollection<ProductVariantResponse>>(
            activeOnly
                ? variants
                    .Where(variant =>
                        variant.Status == ProductVariantStatus.Active)
                    .ToArray()
                : variants);
    }

    public Task<ProductVariantResponse?> GetVariantByIdAsync(
        Guid productId,
        Guid variantId,
        CancellationToken cancellationToken)
    {
        if (productId != ProductId || variantId != VariantId)
        {
            return Task.FromResult<ProductVariantResponse?>(null);
        }

        return Task.FromResult<ProductVariantResponse?>(
            CreateVariant(
                VariantId,
                "500g Box",
                ProductVariantStatus.Active));
    }

    public Task<ProductVariantResponse> CreateVariantAsync(
        Guid productId,
        CreateProductVariantRequest request,
        CancellationToken cancellationToken)
    {
        if (productId != ProductId)
        {
            throw new InvalidOperationException(
                "The specified product does not exist.");
        }

        return Task.FromResult(
            CreateVariant(
                VariantId,
                request.Name,
                ProductVariantStatus.Active));
    }

    public Task<ProductVariantResponse?> UpdateVariantAsync(
        Guid productId,
        Guid variantId,
        UpdateProductVariantRequest request,
        CancellationToken cancellationToken)
    {
        if (productId != ProductId || variantId != VariantId)
        {
            return Task.FromResult<ProductVariantResponse?>(null);
        }

        return Task.FromResult<ProductVariantResponse?>(
            CreateVariant(
                variantId,
                request.Name,
                ProductVariantStatus.Active));
    }

    public Task<bool> DeactivateVariantAsync(
        Guid productId,
        Guid variantId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(
            productId == ProductId &&
            variantId == VariantId);
    }

    private static ProductResponse CreateProduct(
        Guid id,
        string name,
        ProductStatus status)
    {
        var now = DateTime.UtcNow;

        return new ProductResponse(
            id,
            SweetShopApiFactory.TestShopId,
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            name,
            null,
            1,
            status,
            now,
            now);
    }

    private static ProductVariantResponse CreateVariant(
        Guid id,
        string name,
        ProductVariantStatus status)
    {
        var now = DateTime.UtcNow;

        return new ProductVariantResponse(
            id,
            ProductId,
            name,
            500,
            ProductUnit.Kilogram,
            450,
            "INR",
            1,
            status,
            now,
            now);
    }

   public Task<bool> ActivateProductAsync(
    Guid productId,
    CancellationToken cancellationToken)
        {
            return Task.FromResult(
                productId == Guid.Parse("33333333-3333-3333-3333-333333333333") ||
                productId == Guid.Parse("44444444-4444-4444-4444-444444444444"));
        }

public Task<bool> ActivateVariantAsync(
    Guid productId,
    Guid variantId,
    CancellationToken cancellationToken)
        {
            return Task.FromResult(
                productId == Guid.Parse("33333333-3333-3333-3333-333333333333") &&
                (variantId == Guid.Parse("55555555-5555-5555-5555-555555555555") ||
                variantId == Guid.Parse("66666666-6666-6666-6666-666666666666")));
        }
}