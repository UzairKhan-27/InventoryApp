using InventoryApp.Helpers;
using InventoryApp.Models;

namespace InventoryApp.Services
{
    public interface IStoresService
    {
        Task<Store> AddStore(AddStoreDto dto);
        Task<(bool IsSuccess, string Message)> DeleteStore(Guid id);
        Task<List<Store>> GetAllStores();
        Task<Store?> GetStore(Guid id, UserContext userContext);
        Task<Store?> UpdateStore(Guid id, UpdateStoreDto dto);
    }
}