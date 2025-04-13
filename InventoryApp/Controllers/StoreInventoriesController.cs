using InventoryApp.Helpers;
using InventoryApp.Models;
using InventoryApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.Controllers;

[Authorize]
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
            var userContext = new UserContext(User);
            var inventories = await _service.GetStoreInventories(userContext);
            return Ok(inventories);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving store inventories.");
        }
    }


    [HttpGet("{storeId}/{productId}")]
    public async Task<ActionResult<StoreInventory>> GetStoreInventory(Guid storeId, Guid productId)
    {
        try
        {
            var userContext = new UserContext(User);
            var inventory = await _service.GetStoreInventory(storeId, productId, userContext);

            return inventory == null
                ? NotFound($"Inventory not found for StoreID {storeId} and ProductID {productId}")
                : Ok(inventory);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving the store inventory.");
        }
    }


    [HttpGet("store/{storeId}")]
    public async Task<ActionResult<List<StoreInventory>>> GetInventoryByStore(Guid storeId)
    {
        try
        {
            var userContext = new UserContext(User);
            var inventories = await _service.GetInventoryByStore(storeId, userContext);

            return inventories.Count == 0
                ? NotFound($"No inventory found for StoreID {storeId}")
                : Ok(inventories);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, $"An error occurred while retrieving inventories for StoreID {storeId}.");
        }
    }


}
