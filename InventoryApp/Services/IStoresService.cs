using InventoryApp.Helpers;
using InventoryApp.Models;

namespace InventoryApp.Services
{
    public interface IStoresService
    {
        Task<Store> AddStore(AddStoreDto dto, string changedBy);
        Task<(bool IsSuccess, string Message)> DeleteStore(Guid id, string changedBy);
        Task<List<Store>> GetAllStores();
        Task<Store?> GetStore(Guid id, UserContext userContext);
        Task<Store?> UpdateStore(Guid id, UpdateStoreDto dto, string changedBy);
    }
}