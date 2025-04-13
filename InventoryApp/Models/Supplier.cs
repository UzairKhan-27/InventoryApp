using System.Text.Json.Serialization;

namespace InventoryApp.Models;

public class Supplier
{

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public bool IsDeleted { get; set; } = false;

    [JsonIgnore]
    public ICollection<Product> Products { get; set; } 

}
