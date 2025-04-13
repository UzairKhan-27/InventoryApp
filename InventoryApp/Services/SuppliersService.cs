using InventoryApp.Data;
using InventoryApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace InventoryApp.Services;

public class SuppliersService : ISuppliersService
{
    private readonly ProductContext _context;
    private readonly IAuditLogsService _auditLog;
    private readonly IMemoryCache _cache;

    public SuppliersService(ProductContext context, IAuditLogsService auditLog, IMemoryCache cache)
    {
        _context = context;
        _auditLog = auditLog;
        _cache = cache;
    }

    public async Task<List<Supplier>> GetSuppliers()
    {
        var suppliers = await _cache.GetOrCreateAsync("suppliers_list", entry =>
        {
            return _context.Suppliers.Where(s => !s.IsDeleted).ToListAsync();
        });

        return suppliers ?? new List<Supplier>();
    }

    public async Task<Supplier?> GetSupplier(Guid id)
    {
        return await _cache.GetOrCreateAsync($"supplier_{id}", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            return _context.Suppliers.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        });
    }

    public async Task<Supplier> AddSupplier(AddSupplierDto dto, string changedBy)
    {
        var supplier = new Supplier
        {
            Name = dto.Name,
        };

        await _context.Suppliers.AddAsync(supplier);
        await _context.SaveChangesAsync();
        _cache.Remove("suppliers_list");

        await _auditLog.LogChangeAsync(
            entityName: "Supplier",
            entityId: supplier.Id,
            action: "Add",
            changedBy: changedBy,
            details: $"Added supplier: {supplier.Name}"
        );

        return supplier;
    }

    public async Task<Supplier?> UpdateSupplier(Guid id, UpdateSupplierDto dto, string changedBy)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier == null || supplier.IsDeleted)
            return null;

        supplier.Name = dto.Name;

        await _context.SaveChangesAsync();
        _cache.Remove("suppliers_list");
        _cache.Remove($"supplier_{id}");

        await _auditLog.LogChangeAsync(
            entityName: "Supplier",
            entityId: supplier.Id,
            action: "Update",
            changedBy: changedBy,
            details: $"Updated supplier: {supplier.Name}"
        );

        return supplier;
    }

    public async Task<(bool success, string message)> DeleteSupplier(Guid id, string changedBy)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier == null || supplier.IsDeleted)
            return (false, $"Supplier with ID {id} not found");

        supplier.IsDeleted = true;

        await _context.SaveChangesAsync();
        _cache.Remove("suppliers_list");
        _cache.Remove($"supplier_{id}");

        await _auditLog.LogChangeAsync(
            entityName: "Supplier",
            entityId: supplier.Id,
            action: "Delete",
            changedBy: changedBy,
            details: $"Deleted supplier: {supplier.Name}"
        );

        return (true, $"Supplier with ID {id} deleted");
    }
}
