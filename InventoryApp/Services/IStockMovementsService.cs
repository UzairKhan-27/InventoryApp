using InventoryApp.Helpers;
using InventoryApp.Models;
using System.Security.Claims;

namespace InventoryApp.Services
{
    public interface IStockMovementsService
    {
        /*Task<(bool IsSuccess, string? Message, StockMovement? Movement)> AddStockMovement(AddStockMovementDto dto);*/
        Task<(bool isSuccess, string message, StockMovement? stockMovement)> AddStockMovement(AddStockMovementDto dto, UserContext userContext);
        Task<StockMovement?> GetStockMovement(Guid id, UserContext userContext);
        Task<List<StockMovement>> GetStockMovements(UserContext userContext);
        Task<(bool found, string message, List<StockMovement> stockMovement)> GetFilteredStockMovements
            (Guid? storeId, DateTime? startDate, DateTime? endDate, UserContext userContext);

    }
}