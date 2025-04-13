
namespace InventoryApp.Services
{
    public interface IAuditLogsService
    {
        Task LogChangeAsync(string entityName, Guid entityId, string action, string changedBy, string details);
    }
}