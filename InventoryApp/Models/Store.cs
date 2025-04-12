using System.Text.Json.Serialization;

namespace InventoryApp.Models;
public class Store
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Location { get; set; }
    public bool IsDeleted { get; set; } = false;
    [JsonIgnore]
    public ICollection<StoreInventory> StoreInventory { get; set; }
    [JsonIgnore]
    public ICollection<StockMovement> StockMovements { get; set; }

}

