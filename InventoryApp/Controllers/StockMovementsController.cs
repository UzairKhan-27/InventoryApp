using InventoryApp.Helpers;
using InventoryApp.Models;
using InventoryApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class StockMovementsController : ControllerBase
{
    private readonly IStockMovementsService _service;
    public StockMovementsController(IStockMovementsService service)
    {
        _service = service;
    }

    [HttpGet]
    [EnableRateLimiting("ReadPolicy")]
    public async Task<ActionResult<List<StockMovement>>> GetStockMovements()
    {
        try
        {
            var userContext = new UserContext(User);
            var stockMovements = await _service.GetStockMovements(userContext);
            return Ok(stockMovements);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving stock movements.");
        }
    }



    [HttpGet("{id}")]
    [EnableRateLimiting("ReadPolicy")]
    public async Task<ActionResult<StockMovement>> GetStockMovement(Guid id)
    {
        try
        {
            var userContext = new UserContext(User);
            var stockMovement = await _service.GetStockMovement(id, userContext);

            return stockMovement == null
                ? NotFound($"Stock movement with ID {id} not found.")
                : Ok(stockMovement);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving the stock movement.");
        }
    }


    [EnableRateLimiting("WritePolicy")]
    [HttpPost]
    public async Task<ActionResult<StockMovement>> AddStockMovement([FromBody] AddStockMovementDto dto)
    {
        try
        {
            var userContext = new UserContext(User);
            var (isSuccess, message, stockMovement) = await _service.AddStockMovement(dto, userContext);

            return isSuccess ? Ok(stockMovement) : BadRequest(message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while adding stock movement.");
        }
    }

    [HttpGet("filter")]
    [EnableRateLimiting("ReadPolicy")]
    public async Task<ActionResult<List<StockMovement>>> GetFilteredStockMovements
    ([FromQuery] Guid? storeId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            var userContext = new UserContext(User);
            var result = await _service.GetFilteredStockMovements(storeId, startDate, endDate, userContext);

            if (!result.found)
                return NotFound(result.message);

            return Ok(result.stockMovement);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);  
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving filtered stock movements.");
        }
    }



}
