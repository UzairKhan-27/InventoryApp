using InventoryApp.Models;

namespace InventoryApp.Services
{
    public interface IStockMovementsService
    {
        /*Task<(bool IsSuccess, string? Message, StockMovement? Movement)> AddStockMovement(AddStockMovementDto dto);*/
        Task<(bool isSuccess, string message, StockMovement? stockMovement)> AddStockMovement(AddStockMovementDto dto);
        Task<StockMovement?> GetStockMovement(Guid id);
        Task<List<StockMovement>> GetStockMovements();
        Task<(bool found, string message, List<StockMovement> stockMovement)> GetFilteredStockMovements(Guid? storeId, DateTime? startDate, DateTime? endDate);

    }
}