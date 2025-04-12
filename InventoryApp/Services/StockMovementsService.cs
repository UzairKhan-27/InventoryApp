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

    public async Task<(bool IsSuccess, string? Message, StockMovement? Movement)> AddStockMovement(AddStockMovementDto dto)
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
    }

    private (bool IsValid, string? ErrorMessage) HandleStockChange(Product product, string type, int count)
    {
        switch (type)
        {
            case "stocked in":
                /*product.Quantity += count;*/
                return (true, null);

            case "sold":
            case "removed":
                /*if (count > product.Quantity)
                    return (false, "Cannot remove or sell more than available stock.");
                product.Quantity -= count;*/
                return (true, null);

            default:
                return (false, "Invalid stock movement type. Use 'stocked in', 'sold', or 'removed'.");
        }
    }


}
