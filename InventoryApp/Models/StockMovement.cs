using InventoryApp.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class StockMovement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public Guid StoreId { get; set; }
    [RegularExpression("^(stocked in|sold|removed)$", ErrorMessage = "Choose between stocked in, sold and removed")]
    public string Type { get; set; }
    public int Count { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    [JsonIgnore]
    public Product Product { get; set; }
    [JsonIgnore]
    public Store Store { get; set; }
}
