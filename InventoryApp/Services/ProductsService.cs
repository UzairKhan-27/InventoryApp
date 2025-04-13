using InventoryApp.Data;
using InventoryApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace InventoryApp.Services;

public class ProductsService : IProductsService
{
    private readonly ProductContext _context;
    private readonly IAuditLogsService _auditLog;
    private readonly IMemoryCache _cache;


    public ProductsService(ProductContext context, IAuditLogsService auditLog,IMemoryCache cache)
    {
        _context = context;
        _auditLog = auditLog;
        _cache = cache;
    }

    public async Task<List<Product>> GetProducts()
    {
        var products = await _cache.GetOrCreateAsync("products_list", entry =>
        {
            return _context.Products.Where(p => !p.IsDeleted).ToListAsync();
        });

        return products ?? new List<Product>();
    }

    public async Task<Product?> GetProduct(Guid id)
    {
        return await _cache.GetOrCreateAsync($"product_{id}", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            return _context.Products.Where(p => p.Id == id && !p.IsDeleted).FirstOrDefaultAsync();
        });
    }


    public async Task<Product?> AddProduct(AddProductDto dto, string changedBy)
    {

        var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == dto.SupplierId && !s.IsDeleted);
        if (!supplierExists)
            return null;

        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            SupplierId = dto.SupplierId
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        _cache.Remove("products_list");
        await _auditLog.LogChangeAsync(
            entityName: "Product",
            entityId: product.Id,
            action: "Add",
            changedBy: changedBy,
            details: $"Added product: {product.Name}"
        );
        return product;
    }

    public async Task<Product?> UpdateProduct(Guid id, UpdateProductDto dto, string changedBy)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null || product.IsDeleted)
            return null;

        var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == dto.SupplierId && !s.IsDeleted);
        if (!supplierExists)
            return null;

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _cache.Remove("products_list");
        await _auditLog.LogChangeAsync(
            entityName: "Product",
            entityId: product.Id,
            action: "Update",
            changedBy: changedBy,
            details: $"Updated product: {product.Name}"
        );
        return product;
    }

    public async Task<(bool success, string message)> DeleteProduct(Guid id, string changedBy)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null || product.IsDeleted)
            return (false, $"Product with {id} not found");

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        _cache.Remove("products_list");
        await _auditLog.LogChangeAsync(
            entityName: "Product",
            entityId: product.Id,
            action: "Delete",
            changedBy: changedBy,
            details: $"Deleted product: {product.Name}"
        );


        return (true, $"Product with {id} deleted");
    }

}

