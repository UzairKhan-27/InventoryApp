using InventoryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Data;

public class ProductContext : DbContext
{
    public ProductContext(DbContextOptions<ProductContext> options) : base(options)
    {
    }
    public DbSet<Product> Products { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }

}
