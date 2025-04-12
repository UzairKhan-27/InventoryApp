namespace InventoryApp.Models;
public class Store
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Location { get; set; }
    public bool IsDeleted { get; set; } = false;
    public ICollection<StoreInventory> StoreInventory { get; set; }
    public ICollection<StockMovement> StockMovements { get; set; }

}

