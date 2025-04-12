using System.Text.Json.Serialization;

namespace InventoryApp.Models;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public int Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    [JsonIgnore]
    public ICollection<StoreInventory> StoreInventory { get; set; }
    [JsonIgnore]
    public ICollection<StockMovement> StockMovements { get; set; }
}
