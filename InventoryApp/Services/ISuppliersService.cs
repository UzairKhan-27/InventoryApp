using InventoryApp.Models;

namespace InventoryApp.Services
{
    public interface ISuppliersService
    {
        Task<Supplier> AddSupplier(AddSupplierDto dto, string changedBy);
        Task<(bool success, string message)> DeleteSupplier(Guid id, string changedBy);
        Task<Supplier?> GetSupplier(Guid id);
        Task<List<Supplier>> GetSuppliers();
        Task<Supplier?> UpdateSupplier(Guid id, UpdateSupplierDto dto, string changedBy);
    }
}