using InventoryApp.Models;

namespace InventoryApp.Services
{
    public interface IAuthenticationService
    {
        string GenerateJwtToken(User user);
        Task<(bool success, string? token, string message)> Login(string username, string password);
        Task<(bool success, string message)> Register(string username, string password, string role, Guid? storeId);
    }
}