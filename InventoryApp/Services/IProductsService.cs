using InventoryApp.Models;

namespace InventoryApp.Services
{
    public interface IProductsService
    {
        Task<Product> AddProduct(AddProductDto dto, string changedBy);
        Task<(bool success, string message)> DeleteProduct(Guid id, string changedBy);
        Task<Product?> GetProduct(Guid id);
        Task<List<Product>> GetProducts();
        Task<Product?> UpdateProduct(Guid id, UpdateProductDto dto, string changedBy);
    }
}