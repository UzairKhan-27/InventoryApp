namespace InventoryApp;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EntityName { get; set; }
    public Guid EntityId { get; set; }
    public string Action { get; set; }
    public string ChangedBy { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Details { get; set; }

}
