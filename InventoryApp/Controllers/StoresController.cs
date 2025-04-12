using InventoryApp.Models;
using InventoryApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StoresController : ControllerBase
{
    private readonly IStoresService _service;

    public StoresController(IStoresService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<Store>>> GetAllStores()
    {
        try
        {
            var stores = await _service.GetAllStores();
            return Ok(stores);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching stores: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Store>> GetStoreById(Guid id)
    {
        try
        {
            var store = await _service.GetStore(id);
            return store == null
                ? NotFound($"Store with ID {id} not found.")
                : Ok(store);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching the store: {ex.Message}");
        }
    }
    [HttpPost]
    public async Task<ActionResult<Store>> AddStore([FromBody] AddStoreDto dto)
    {
        try
        {
            var store = await _service.AddStore(dto);
            return Ok(store);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while adding the store.");
        }
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<Store>> UpdateStore(Guid id, [FromBody] UpdateStoreDto dto)
    {
        try
        {
            var store = await _service.UpdateStore(id, dto);
            return store == null ? NotFound($"Store with ID {id} not found.") : Ok(store);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the store with ID {id}.");
        }
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteStore(Guid id)
    {
        try
        {
            var (isSuccess, message) = await _service.DeleteStore(id);
            return isSuccess ? Ok(message) : BadRequest(message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting store with ID {id}.");
        }
    }


}
