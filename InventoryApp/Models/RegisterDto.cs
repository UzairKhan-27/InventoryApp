namespace InventoryApp.Models;

public class RegisterDto
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } // "CentralAdmin" or "StoreAdmin"
    public Guid? StoreId { get; set; } // Optional for CentralAdmin
}
