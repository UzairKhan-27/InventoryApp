using InventoryApp.Data;
using InventoryApp.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace InventoryApp.Services;

public class StoreInventoriesService : IStoreInventoriesService
{
    private readonly ProductContext _context;
    private readonly IMemoryCache _cache;

    public StoreInventoriesService(ProductContext context,IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<StoreInventory>> GetStoreInventories(UserContext userContext)
    {
        if (userContext.IsCentralAdmin)
        {
            return await _cache.GetOrCreateAsync("store_inventories_all", entry =>
            {
                return _context.StoreInventories.ToListAsync();
            }) ?? new List<StoreInventory>();
        }

        else if (userContext.IsStoreAdmin && userContext.StoreId.HasValue)
        {
            return await _cache.GetOrCreateAsync($"store_inventories_store_{userContext.StoreId.Value}", entry =>
            {
                return _context.StoreInventories.Where(si => si.StoreId == userContext.StoreId.Value).ToListAsync();
            }) ?? new List<StoreInventory>();
        }

        throw new UnauthorizedAccessException("Access denied: Invalid role or missing store ID.");
    }



    public async Task<StoreInventory?> GetStoreInventory(Guid storeId, Guid productId, UserContext userContext)
    {
        if (userContext.IsStoreAdmin && userContext.StoreId != storeId)
        {
            throw new UnauthorizedAccessException("Access denied: You are not authorized to view this inventory.");
        }

        var result = await _cache.GetOrCreateAsync($"store_inventory_{storeId}_{productId}", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            return _context.StoreInventories.FirstOrDefaultAsync(si => si.StoreId == storeId && si.ProductId == productId);
        });

        return result;
    }


    public async Task<List<StoreInventory>> GetInventoryByStore(Guid storeId, UserContext userContext)
    {
        if (userContext.IsStoreAdmin && userContext.StoreId != storeId)
        {
            throw new UnauthorizedAccessException("Access denied: You are not authorized to view inventory for this store.");
        }

        var inventories = await _cache.GetOrCreateAsync($"store_inventories_store_{userContext.StoreId}", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            return _context.StoreInventories.Where(si => si.StoreId == storeId).ToListAsync();
        });

        return inventories ?? new List<StoreInventory>();
    }




}
