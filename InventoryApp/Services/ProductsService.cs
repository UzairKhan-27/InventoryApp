using InventoryApp.Data;
using InventoryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Services;

public class ProductsService : IProductsService
{
    private readonly ProductContext _context;
    private readonly IAuditLogsService _auditLog;

    public ProductsService(ProductContext context, IAuditLogsService auditLog)
    {
        _context = context;
        _auditLog = auditLog;
    }

    public async Task<List<Product>> GetProducts()
    {
        return await _context.Products.Where(p => !p.IsDeleted).ToListAsync();
    }

    public async Task<Product?> GetProduct(Guid id)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<Product> AddProduct(AddProductDto dto, string changedBy)
    {
        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
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

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
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

