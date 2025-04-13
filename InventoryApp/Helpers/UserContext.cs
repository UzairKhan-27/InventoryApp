using System.Security.Claims;
namespace InventoryApp.Helpers;

public class UserContext
{
    public string Role { get; }
    public Guid? StoreId { get; }
    public Guid UserId { get; }

    public UserContext(ClaimsPrincipal user)
    {
        Role = user.FindFirst(ClaimTypes.Role)?.Value ?? "";
        UserId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        StoreId = Guid.TryParse(user.FindFirst("StoreId")?.Value, out Guid id) ? id : (Guid?)null;
    }

    public bool IsCentralAdmin => Role == "CentralAdmin";
    public bool IsStoreAdmin => Role == "StoreAdmin";
}

