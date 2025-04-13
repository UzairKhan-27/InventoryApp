using InventoryApp.Data;
using InventoryApp.Helpers;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Services;

public class StoreInventoriesService : IStoreInventoriesService
{
    private readonly ProductContext _context;

    public StoreInventoriesService(ProductContext context)
    {
        _context = context;
    }

    public async Task<List<StoreInventory>> GetStoreInventories(UserContext userContext)
    {
        if (userContext.IsCentralAdmin)
        {
            return await _context.StoreInventories.ToListAsync();
        }
        else if (userContext.IsStoreAdmin && userContext.StoreId.HasValue)
        {
            return await _context.StoreInventories
                .Where(si => si.StoreId == userContext.StoreId.Value)
                .ToListAsync();
        }

        throw new UnauthorizedAccessException("Access denied: Invalid role or missing store ID.");
    }


    public async Task<StoreInventory?> GetStoreInventory(Guid storeId, Guid productId, UserContext userContext)
    {
        if (userContext.IsStoreAdmin && userContext.StoreId != storeId)
        {
            throw new UnauthorizedAccessException("Access denied: You are not authorized to view this inventory.");
        }

        return await _context.StoreInventories
            .FirstOrDefaultAsync(si => si.StoreId == storeId && si.ProductId == productId);
    }


    public async Task<List<StoreInventory>> GetInventoryByStore(Guid storeId, UserContext userContext)
    {
        if (userContext.IsStoreAdmin && userContext.StoreId != storeId)
        {
            throw new UnauthorizedAccessException("Access denied: You are not authorized to view inventory for this store.");
        }

        return await _context.StoreInventories
            .Where(si => si.StoreId == storeId)
            .ToListAsync();
    }



}
