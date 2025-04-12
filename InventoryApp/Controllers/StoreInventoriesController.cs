using InventoryApp.Models;
using InventoryApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StoreInventoriesController : ControllerBase
{
    private readonly IStoreInventoriesService _service;

    public StoreInventoriesController(IStoreInventoriesService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<StoreInventory>>> GetStoreInventories()
    {
        try
        {
            var inventories = await _service.GetStoreInventories();
            return Ok(inventories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while retrieving store inventories.");
        }
    }

    [HttpGet("{storeId}/{productId}")]
    public async Task<ActionResult<StoreInventory>> GetStoreInventory(Guid storeId, Guid productId)
    {
        try
        {
            var inventory = await _service.GetStoreInventory(storeId, productId);
            return inventory == null
                ? NotFound($"Inventory not found for StoreID {storeId} and ProductID {productId}")
                : Ok(inventory);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving the store inventory.");
        }
    }

    [HttpGet("store/{storeId}")]
    public async Task<ActionResult<List<StoreInventory>>> GetInventoryByStore(Guid storeId)
    {
        try
        {
            var inventories = await _service.GetInventoriesByStoreId(storeId);
            return inventories.Count == 0
                ? NotFound($"No inventory found for StoreID {storeId}")
                : Ok(inventories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving inventories for StoreID {storeId}.");
        }
    }

}
