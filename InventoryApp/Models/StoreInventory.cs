using InventoryApp.Models;
using System.Text.Json.Serialization;

public class StoreInventory
{
    public Guid StoreId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    [JsonIgnore]
    public Store Store { get; set; }
    [JsonIgnore]
    public Product Product { get; set; }
}
