using InventoryApp.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Services;

public class StoreInventoriesService : IStoreInventoriesService
{
    private readonly ProductContext _context;

    public StoreInventoriesService(ProductContext context)
    {
        _context = context;
    }

    public async Task<List<StoreInventory>> GetStoreInventories()
    {
        return await _context.StoreInventories.ToListAsync();
    }

    public async Task<StoreInventory?> GetStoreInventory(Guid storeId, Guid productId)
    {
        return await _context.StoreInventories
            .FirstOrDefaultAsync(si => si.StoreId == storeId && si.ProductId == productId);
    }

    public async Task<List<StoreInventory>> GetInventoriesByStoreId(Guid storeId)
    {
        return await _context.StoreInventories.Where(si => si.StoreId == storeId).ToListAsync();
    }


}
