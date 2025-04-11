using InventoryApp.Models;
using InventoryApp.Services;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<List<StockMovement>>> GetStockMovements()
    {
        try
        {
            var stockMovements = await _service.GetStockMovements();
            return Ok(stockMovements);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while retrieving stock movements.");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StockMovement>> GetStockMovement(Guid id)
    {
        try
        {
            var stockMovement = await _service.GetStockMovement(id);
            return stockMovement == null
                ? NotFound($"Stock movement with ID {id} not found.")
                : Ok(stockMovement);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while retrieving the stock movement.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<StockMovement>> AddStockMovement([FromBody] AddStockMovementDto dto)
    {
        try
        {
            var (isSuccess, message, movement) = await _service.AddStockMovement(dto);
            return isSuccess ? Ok(movement) : BadRequest(message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while adding the stock movement.");
        }
    }
}
