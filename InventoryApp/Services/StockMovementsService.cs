using InventoryApp.Data;
using InventoryApp.Helpers;
using InventoryApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace InventoryApp.Services;

public class StockMovementsService : IStockMovementsService
{
    private readonly ProductContext _context;
    private readonly IAuditLogsService _auditLog;
    private readonly IMemoryCache _cache;


    public StockMovementsService(ProductContext context, IAuditLogsService auditLog, IMemoryCache cache)
    {
        _context = context;
        _auditLog = auditLog;
        _cache = cache;
    }

    public async Task<List<StockMovement>> GetStockMovements(UserContext userContext)
    {
        if (userContext.IsCentralAdmin)
        {
            return await _cache.GetOrCreateAsync("stock_movements_all", entry =>
            {
                return _context.StockMovements.ToListAsync();
            }) ?? new List<StockMovement>();
        }
        else if (userContext.IsStoreAdmin && userContext.StoreId.HasValue)
        {
            var cacheKey = $"stock_movements_store_{userContext.StoreId.Value}";
            return await _cache.GetOrCreateAsync(cacheKey, entry =>
            {
                return _context.StockMovements.Where(sm => sm.StoreId == userContext.StoreId.Value).ToListAsync();
            }) ?? new List<StockMovement>();
        }

        throw new UnauthorizedAccessException("Invalid role or missing store ID.");
    }


    public async Task<StockMovement?> GetStockMovement(Guid id, UserContext userContext)
    {
        var stockMovement = await _context.StockMovements.FindAsync(id);
        if (stockMovement == null)
            return null;

        if (userContext.IsCentralAdmin)
            return stockMovement;

        if (userContext.IsStoreAdmin && userContext.StoreId.HasValue && stockMovement.StoreId == userContext.StoreId.Value)
            return stockMovement;

        throw new UnauthorizedAccessException("You are not authorized to view this stock movement.");
    }

    public async Task<(bool isSuccess, string message, StockMovement? stockMovement)> AddStockMovement(AddStockMovementDto dto, UserContext userContext)
    {
        if (!userContext.IsStoreAdmin || userContext.StoreId != dto.StoreId)
        {
            throw new UnauthorizedAccessException("You are not authorized to perform this action for this store.");
        }

        var countValidation = IsCountPositive(dto.Count);
        if (!countValidation.isValid)
            return (false, countValidation.message, null);

        var storeInventory = await _context.StoreInventories
            .FirstOrDefaultAsync(si => si.StoreId == dto.StoreId && si.ProductId == dto.ProductId);

        var stockValidation = IsStockEnough(dto.Type, storeInventory, dto.Count);
        if (!stockValidation.isValid)
            return (false, stockValidation.message, null);

        if (storeInventory == null)
        {
            storeInventory = new StoreInventory
            {
                StoreId = dto.StoreId,
                ProductId = dto.ProductId,
                Quantity = 0,
                CreatedAt = DateTime.UtcNow
            };
            await _context.StoreInventories.AddAsync(storeInventory);
        }

        if (dto.Type == "stocked in")
            storeInventory.Quantity += dto.Count;
        else if (dto.Type == "sold" || dto.Type == "removed")
            storeInventory.Quantity -= dto.Count;

        storeInventory.UpdatedAt = DateTime.UtcNow;

        var stockMovement = new StockMovement
        {
            StoreId = dto.StoreId,
            ProductId = dto.ProductId,
            Type = dto.Type,
            Count = dto.Count
        };

        await _context.StockMovements.AddAsync(stockMovement);
        await _context.SaveChangesAsync();
        _cache.Remove("stock_movements_all");
        _cache.Remove($"stock_movements_store_{userContext.StoreId.Value}");
        _cache.Remove($"store_inventories_all");
        _cache.Remove($"store_inventories_store_{userContext.StoreId.Value}");


        await _auditLog.LogChangeAsync(
            entityName: "StockMovement",
            entityId: stockMovement.Id,
            action: "Add",
            changedBy: userContext.UserId.ToString(),
            details: $"StockMovement: {dto.Type} | ProductId: {dto.ProductId} | Count: {dto.Count} | StoreId: {dto.StoreId}"
        );

        await _auditLog.LogChangeAsync(
            entityName: "StoreInventory",
            entityId: storeInventory.StoreId,
            action: "Update",
            changedBy: userContext.UserId.ToString(),
            details: $"Inventory updated after {dto.Type} | ProductId: {dto.ProductId} | New Quantity: {storeInventory.Quantity}"
        );


        return (true, "Stock movement recorded successfully.", stockMovement);
    }


    public async Task<(bool found, string message, List<StockMovement> stockMovement)> GetFilteredStockMovements
    (Guid? storeId, DateTime? startDate, DateTime? endDate, UserContext userContext)
    {
        var query = _context.StockMovements.AsQueryable();

        if (userContext.IsStoreAdmin && storeId.HasValue && userContext.StoreId != storeId)
        {
            throw new UnauthorizedAccessException("You are not authorized to view stock movements for this store.");
        }

        if (storeId.HasValue)
            query = query.Where(sm => sm.StoreId == storeId.Value);
        if (startDate.HasValue)
            query = query.Where(sm => sm.Timestamp >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(sm => sm.Timestamp <= endDate.Value);

        var results = await query.ToListAsync();

        return results.Count == 0
            ? (false, "No stock movements found for the given filters.", results)
            : (true, "Stock movements retrieved successfully.", results);
    }


    private (bool isValid, string message) IsCountPositive(int count)
    {
        if (count <= 0)
            return (false, "Count must be greater than 0.");
        return (true, "Count greater than 0");
    }

    private (bool isValid, string message) IsStockEnough(string type, StoreInventory? storeInventory, int count)
    {
        if ((type == "sold" || type == "removed") && 
            (storeInventory == null || storeInventory.Quantity < count))
        {
            return (false, "Not enough stock available to perform this operation.");
        }

        return (true, "Enough stock.");
    }


}
