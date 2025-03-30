using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Models;

public class StockMovement
{
    public Guid Id { get; set; }
    [RegularExpression("^(stocked in|sold|removed)$",ErrorMessage ="Chooose between stocked in, sold and removed")]
    public Guid ProductId {get; set;}
    public string Type { get; set; }
    public int Count { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public Product Product { get; set;}


}
