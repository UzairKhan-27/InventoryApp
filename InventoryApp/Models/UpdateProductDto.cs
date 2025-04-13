namespace InventoryApp.Models;

public class UpdateProductDto
{
    public string Name { get; set; }
    public int Price { get; set; }
    public Guid SupplierId { get; set; }

}
