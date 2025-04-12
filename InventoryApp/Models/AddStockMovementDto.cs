using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Models;

public class AddStockMovementDto
{
    public Guid StoreId { get; set; }
    public Guid ProductId { get; set; }
    [RegularExpression("^(stocked in|sold|removed)$", ErrorMessage = "Chooose between stocked in, sold and removed")]
    public string Type { get; set; }
    public int Count { get; set; }
}
