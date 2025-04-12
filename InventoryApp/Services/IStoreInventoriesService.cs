
namespace InventoryApp.Services
{
    public interface IStoreInventoriesService
    {
        Task<List<StoreInventory>> GetInventoriesByStoreId(Guid storeId);
        Task<List<StoreInventory>> GetStoreInventories();
        Task<StoreInventory?> GetStoreInventory(Guid storeId, Guid productId);
    }
}