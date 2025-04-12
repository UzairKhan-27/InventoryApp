using InventoryApp.Data;
using InventoryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Services;

public class ProductsService : IProductsService
{
    private readonly ProductContext _context;

    public ProductsService(ProductContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetProducts()
    {
        return await _context.Products.Where(p => !p.IsDeleted).ToListAsync();
    }

    public async Task<Product?> GetProduct(Guid id)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<Product> AddProduct(AddProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> UpdateProduct(Guid id, UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null || product.IsDeleted)
            return null;

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<(bool IsSuccess, string Message)> DeleteProduct(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null || product.IsDeleted)
            return (false, $"Product with {id} not found");

        /*if (product.Quantity > 0)
            return (false, "Cannot delete product with remaining stock.");*/

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return (true, $"Product with {id} deleted");
    }

}

