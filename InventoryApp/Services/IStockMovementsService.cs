using InventoryApp.Models;

namespace InventoryApp.Services
{
    public interface IStockMovementsService
    {
        Task<(bool IsSuccess, string? Message, StockMovement? Movement)> AddStockMovement(AddStockMovementDto dto);
        Task<StockMovement?> GetStockMovement(Guid id);
        Task<List<StockMovement>> GetStockMovements();
    }
}