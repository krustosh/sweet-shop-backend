using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Api.Common.Models;
using SweetShop.Domain.Enums;
using SweetShop.Application.Features.Inventory;
using SweetShop.Application.Features.Inventory.Requests;
using SweetShop.Application.Features.Inventory.Responses;

namespace SweetShop.Api.Controllers;

/// <summary>
/// Provides administrative APIs for managing product inventory.
/// </summary>
[ApiController]
[Authorize(Roles = nameof(UserRole.Admin))]
[Route("api/v1/admin/inventory")]
public sealed class InventoryController : ControllerBase
{
    private readonly IInventoryService inventoryService;

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryController"/> class.
    /// </summary>
    /// <param name="inventoryService">
    /// The inventory application service.
    /// </param>
    public InventoryController(IInventoryService inventoryService)
    {
        ArgumentNullException.ThrowIfNull(inventoryService);
        this.inventoryService = inventoryService;
    }

    /// <summary>
    /// Gets all inventory records for the current shop.
    /// </summary>
    /// <param name="cancellationToken">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// The inventory records.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<IReadOnlyCollection<InventoryResponse>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInventory(
        CancellationToken cancellationToken)
    {
        var response = await inventoryService.GetInventoryAsync(
            cancellationToken);

        return Ok(
            new ApiResponse<IReadOnlyCollection<InventoryResponse>>(
                response));
    }

    /// <summary>
    /// Gets inventory for a specific product variant.
    /// </summary>
    /// <param name="productVariantId">
    /// The product variant identifier.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// The inventory record when found.
    /// </returns>
    [HttpGet("{productVariantId:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse<InventoryResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInventoryByVariantId(
        Guid productVariantId,
        CancellationToken cancellationToken)
    {
        var response = await inventoryService.GetInventoryByVariantIdAsync(
            productVariantId,
            cancellationToken);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(
            new ApiResponse<InventoryResponse>(
                response));
    }

    /// <summary>
    /// Creates an inventory record for a product variant.
    /// </summary>
    /// <param name="productVariantId">
    /// The product variant identifier.
    /// </param>
    /// <param name="request">
    /// The inventory creation request.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// The newly created inventory record.
    /// </returns>
    [HttpPost("{productVariantId:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse<InventoryResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateInventory(
        Guid productVariantId,
        CreateInventoryRequest request,
        CancellationToken cancellationToken)
    {
        var response = await inventoryService.CreateInventoryAsync(
            productVariantId,
            request,
            cancellationToken);

        return Ok(
            new ApiResponse<InventoryResponse>(
                response));
    }

    /// <summary>
    /// Updates the low-stock threshold for a product variant.
    /// </summary>
    /// <param name="productVariantId">
    /// The product variant identifier.
    /// </param>
    /// <param name="lowStockThreshold">
    /// The new low-stock threshold.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// The updated inventory record when found.
    /// </returns>
    [HttpPut("{productVariantId:guid}/threshold")]
    [ProducesResponseType(
        typeof(ApiResponse<InventoryResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateThreshold(
        Guid productVariantId,
        [FromBody] decimal lowStockThreshold,
        CancellationToken cancellationToken)
    {
        var response =
            await inventoryService.UpdateInventoryThresholdAsync(
                productVariantId,
                lowStockThreshold,
                cancellationToken);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(
            new ApiResponse<InventoryResponse>(
                response));
    }

    /// <summary>
    /// Adds available stock to a product variant inventory.
    /// </summary>
    /// <param name="productVariantId">
    /// The product variant identifier.
    /// </param>
    /// <param name="request">
    /// The stock transaction request.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// The updated inventory record.
    /// </returns>
    [HttpPost("{productVariantId:guid}/stock/add")]
    [ProducesResponseType(
        typeof(ApiResponse<InventoryResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddStock(
        Guid productVariantId,
        CreateInventoryTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var createdBy = GetCurrentUserId();

        var response = await inventoryService.AddStockAsync(
            productVariantId,
            request,
            createdBy,
            cancellationToken);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(
            new ApiResponse<InventoryResponse>(
                response));
    }

    /// <summary>
    /// Removes available stock from a product variant inventory.
    /// </summary>
    /// <param name="productVariantId">
    /// The product variant identifier.
    /// </param>
    /// <param name="request">
    /// The stock transaction request.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// The updated inventory record.
    /// </returns>
    [HttpPost("{productVariantId:guid}/stock/remove")]
    [ProducesResponseType(
        typeof(ApiResponse<InventoryResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveStock(
        Guid productVariantId,
        CreateInventoryTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var createdBy = GetCurrentUserId();

        var response = await inventoryService.RemoveStockAsync(
            productVariantId,
            request,
            createdBy,
            cancellationToken);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(
            new ApiResponse<InventoryResponse>(
                response));
    }

    /// <summary>
    /// Gets the inventory transaction history for a product variant.
    /// </summary>
    /// <param name="productVariantId">
    /// The product variant identifier.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// The inventory transaction history.
    /// </returns>
    [HttpGet("{productVariantId:guid}/transactions")]
    [ProducesResponseType(
        typeof(ApiResponse<IReadOnlyCollection<InventoryTransactionResponse>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactions(
        Guid productVariantId,
        CancellationToken cancellationToken)
    {
        var response = await inventoryService.GetTransactionsAsync(
            productVariantId,
            cancellationToken);

        return Ok(
            new ApiResponse<IReadOnlyCollection<InventoryTransactionResponse>>(
                response));
    }

    /// <summary>
    /// Gets the identifier of the currently authenticated administrator.
    /// </summary>
    /// <returns>
    /// The authenticated user identifier.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the authenticated user identifier is missing or invalid.
    /// </exception>
    private Guid GetCurrentUserId()
    {
        var value = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(value, out var userId))
        {
            throw new InvalidOperationException(
                "Authenticated user ID is missing or invalid.");
        }

        return userId;
    }
}