using InventoryApp.Helpers;
using InventoryApp.Models;
using InventoryApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InventoryApp.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SuppliersController : ControllerBase
{
    private readonly ISuppliersService _service;

    public SuppliersController(ISuppliersService service)
    {
        _service = service;
    }

    [HttpGet]
    [EnableRateLimiting("ReadPolicy")]
    public async Task<ActionResult<List<Supplier>>> GetSuppliers()
    {
        try
        {
            var suppliers = await _service.GetSuppliers();
            return Ok(suppliers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while retrieving suppliers.");
        }
    }

    [HttpGet("{id}")]
    [EnableRateLimiting("ReadPolicy")]
    public async Task<ActionResult<Supplier>> GetSupplier(Guid id)
    {
        try
        {
            var supplier = await _service.GetSupplier(id);
            return supplier == null ? NotFound($"Supplier with {id} not found") : Ok(supplier);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving supplier with ID {id}.");
        }
    }

    [Authorize(Roles = "CentralAdmin")]
    [EnableRateLimiting("WritePolicy")]
    [HttpPost]
    public async Task<ActionResult<Supplier>> AddSupplier([FromBody] AddSupplierDto dto)
    {
        try
        {
            var userContext = new UserContext(User);
            string changedBy = userContext.UserId.ToString();
            var supplier = await _service.AddSupplier(dto, changedBy);

            return Ok(supplier);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while adding the supplier.");
        }
    }

    [Authorize(Roles = "CentralAdmin")]
    [EnableRateLimiting("WritePolicy")]
    [HttpPut("{id}")]
    public async Task<ActionResult<Supplier>> UpdateSupplier(Guid id, [FromBody] UpdateSupplierDto dto)
    {
        try
        {
            var userContext = new UserContext(User);
            string changedBy = userContext.UserId.ToString();
            var supplier = await _service.UpdateSupplier(id, dto, changedBy);

            return supplier == null ? NotFound($"Supplier with {id} not found") : Ok(supplier);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating supplier with ID {id}.");
        }
    }

    [Authorize(Roles = "CentralAdmin")]
    [EnableRateLimiting("WritePolicy")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSupplier(Guid id)
    {
        try
        {
            var userContext = new UserContext(User);
            string changedBy = userContext.UserId.ToString();
            var (success, message) = await _service.DeleteSupplier(id, changedBy);
            return success ? Ok(message) : BadRequest(message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting supplier with ID {id}.");
        }
    }
}
