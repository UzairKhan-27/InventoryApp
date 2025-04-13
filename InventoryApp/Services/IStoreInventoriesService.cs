
using InventoryApp.Helpers;

namespace InventoryApp.Services
{
    public interface IStoreInventoriesService
    {
        Task<List<StoreInventory>> GetInventoryByStore(Guid storeId, UserContext userContext);
        Task<List<StoreInventory>> GetStoreInventories(UserContext userContext);
        Task<StoreInventory?> GetStoreInventory(Guid storeId, Guid productId, UserContext userContext);
    }
}