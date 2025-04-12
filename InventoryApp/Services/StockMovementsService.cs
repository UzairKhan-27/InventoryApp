using InventoryApp.Data;
using InventoryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Services;

public class StockMovementsService : IStockMovementsService
{
    private readonly ProductContext _context;

    public StockMovementsService(ProductContext context)
    {
        _context = context;
    }

    public async Task<List<StockMovement>> GetStockMovements()
    {
        return await _context.StockMovements.ToListAsync();
    }

    public async Task<StockMovement?> GetStockMovement(Guid id)
    {
        return await _context.StockMovements.FindAsync(id);
    }

    /*public async Task<(bool IsSuccess, string? Message, StockMovement? Movement)> AddStockMovement(AddStockMovementDto dto)
    {
        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null || product.IsDeleted)
            return (false, $"Product with ID {dto.ProductId} not found.", null);

        if (dto.Count <= 0)
            return (false, "Count must be greater than 0.", null);

        var stockMovement = new StockMovement
        {
            ProductId = dto.ProductId,
            Type = dto.Type,
            Count = dto.Count
        };

        var (isValid, errorMessage) = HandleStockChange(product, dto.Type, dto.Count);
        if (!isValid)
            return (false, errorMessage, null);

        product.UpdatedAt = DateTime.UtcNow;

        await _context.StockMovements.AddAsync(stockMovement);
        await _context.SaveChangesAsync();

        return (true, null, stockMovement);
    }*/

    /*    private (bool IsValid, string? ErrorMessage) HandleStockChange(Product product, string type, int count)
        {
            switch (type)
            {
                case "stocked in":
                    product.Quantity += count;
                    return (true, null);

                case "sold":
                case "removed":
                    if (count > product.Quantity)
                        return (false, "Cannot remove or sell more than available stock.");
                    product.Quantity -= count;
                    return (true, null);

                default:
                    return (false, "Invalid stock movement type. Use 'stocked in', 'sold', or 'removed'.");
            }
        }
    */

    public async Task<(bool isSuccess, string message, StockMovement? stockMovement)> AddStockMovement(AddStockMovementDto dto)
    {
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

        return (true, "Stock movement recorded successfully.", stockMovement);
    }

    public async Task<(bool found, string message, List<StockMovement> stockMovement)> GetFilteredStockMovements
        (Guid? storeId, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.StockMovements.AsQueryable();

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
