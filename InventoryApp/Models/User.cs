using System.Text.Json.Serialization;

namespace InventoryApp.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; } // "CentralAdmin" or "StoreAdmin"
    public Guid? StoreId { get; set; } // null for CentralAdmin
}

