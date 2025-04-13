using InventoryApp.Data;
using InventoryApp.Helpers;
using InventoryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Services;

public class StoresService : IStoresService
{
    private readonly ProductContext _context;
    private readonly IAuditLogsService _auditLog;


    public StoresService(ProductContext context, IAuditLogsService auditLog)
    {
        _context = context;
        _auditLog = auditLog;
    }

    public async Task<List<Store>> GetAllStores()
    {
        return await _context.Stores.Where(s => !s.IsDeleted).ToListAsync();
    }

    public async Task<Store?> GetStore(Guid id, UserContext userContext)
    {
        if (userContext.IsCentralAdmin)
        {
            return await _context.Stores.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }

        if (userContext.IsStoreAdmin && userContext.StoreId == id)
        {
            return await _context.Stores.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }

        throw new UnauthorizedAccessException("You are not authorized to view this store.");
    }



    public async Task<Store> AddStore(AddStoreDto dto, string changedBy)
    {
        var store = new Store
        {
            Name = dto.Name,
            Location = dto.Location
        };

        await _context.Stores.AddAsync(store);
        await _context.SaveChangesAsync();
        await _auditLog.LogChangeAsync(
            entityName: "Store",
            entityId: store.Id,
            action: "Add",
            changedBy: changedBy,
            details: $"Added store: {store.Name} at {store.Location}"
        );
        return store;
    }
    public async Task<Store?> UpdateStore(Guid id, UpdateStoreDto dto, string changedBy)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store == null || store.IsDeleted)
            return null;

        store.Name = dto.Name;
        store.Location = dto.Location;

        await _context.SaveChangesAsync();
        await _auditLog.LogChangeAsync(
           entityName: "Store",
           entityId: store.Id,
           action: "Update",
           changedBy: changedBy,
           details: $"Updated store: {store.Name} at {store.Location}"
       );
        return store;
    }
    public async Task<(bool IsSuccess, string Message)> DeleteStore(Guid id, string changedBy)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store == null || store.IsDeleted)
            return (false, $"Store with ID {id} not found");

        store.IsDeleted = true;
        await _context.SaveChangesAsync();
        await _auditLog.LogChangeAsync(
           entityName: "Store",
           entityId: store.Id,
           action: "Delete",
           changedBy: changedBy,
           details: $"Deleted store: {store.Name} at {store.Location}"
       );
        return (true, $"Store with ID {id} deleted");
    }



}
