using InventoryApp.Data;

namespace InventoryApp.Services;

public class AuditLogsService : IAuditLogsService
{
    private readonly ProductContext _context;

    public AuditLogsService(ProductContext context)
    {
        _context = context;
    }

    public async Task LogChangeAsync(string entityName, Guid entityId, string action, string changedBy, string details)
    {
        var log = new AuditLog
        {
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            ChangedBy = changedBy,
            Timestamp = DateTime.UtcNow,
            Details = details
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
