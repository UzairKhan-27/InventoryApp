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
    public DbSet<Store> Stores { get; set; }
    public DbSet<StoreInventory> StoreInventories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Optional: Composite key config (if needed)
        modelBuilder.Entity<StoreInventory>()
            .HasKey(si => new { si.StoreId, si.ProductId });

        // Optional: Relationship setup
        modelBuilder.Entity<StoreInventory>()
            .HasOne(si => si.Store)
            .WithMany(s => s.StoreInventory)
            .HasForeignKey(si => si.StoreId);

        modelBuilder.Entity<StoreInventory>()
            .HasOne(si => si.Product)
            .WithMany(p => p.StoreInventory)
            .HasForeignKey(si => si.ProductId);

        modelBuilder.Entity<StockMovement>()
            .HasOne(sm => sm.Store)
            .WithMany(s => s.StockMovements)
            .HasForeignKey(sm => sm.StoreId);

        modelBuilder.Entity<StockMovement>()
            .HasOne(sm => sm.Product)
            .WithMany(p => p.StockMovements)
            .HasForeignKey(sm => sm.ProductId);
    }


}
