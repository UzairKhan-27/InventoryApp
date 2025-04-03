using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InventoryApp.Models;

public class StockMovement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    [RegularExpression("^(stocked in|sold|removed)$", ErrorMessage = "Chooose between stocked in, sold and removed")]
    public string Type { get; set; }
    public int Count { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    [JsonIgnore]
    public Product Product { get; set;}


}
