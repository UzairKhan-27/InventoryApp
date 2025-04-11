using InventoryApp.Models;

namespace InventoryApp.Services
{
    public interface IProductsService
    {
        Task<Product> AddProduct(AddProductDto dto);
        Task<(bool IsSuccess, string Message)> DeleteProduct(Guid id);
        Task<Product?> GetProduct(Guid id);
        Task<List<Product>> GetProducts();
        Task<Product?> UpdateProduct(Guid id, UpdateProductDto dto);
    }
}