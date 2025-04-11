using InventoryApp.Data;
using InventoryApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StockMovementsController : ControllerBase
{
    private readonly ProductContext dbContext;

    public StockMovementsController(ProductContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<StockMovement>>> GetStockMovements()
    {
        try
        {
            var stockMovements = await dbContext.StockMovements.ToListAsync();
            return Ok(stockMovements);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving stock movements.");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StockMovement>> GetStockMovement(Guid id)
    {
        try
        {
            var stockMovement = await dbContext.StockMovements.FindAsync(id);
            if (stockMovement == null)
            {
                return NotFound($"Stock movement with ID {id} not found.");
            }
            return Ok(stockMovement);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the stock movement.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<StockMovement>> AddStockMovement([FromBody] AddStockMovementDto addStockMovementDto)
    {
        try
        {
            var product = await dbContext.Products.FindAsync(addStockMovementDto.ProductId);
            if (product == null)
            {
                return NotFound($"Product with ID {addStockMovementDto.ProductId} not found.");
            }

            var stockMovement = new StockMovement()
            {
                ProductId = addStockMovementDto.ProductId,
                Type = addStockMovementDto.Type,
                Count = addStockMovementDto.Count
            };

            switch (stockMovement.Type)
            {
                case "stocked in":
                    product.Quantity += stockMovement.Count;
                    break;
                case "sold":
                case "removed":
                    product.Quantity -= stockMovement.Count;
                    break;
                default:
                    return BadRequest("Invalid stock movement type. Please choose between 'stocked in', 'sold' or 'removed'.");
            }

            await dbContext.StockMovements.AddAsync(stockMovement);
            await dbContext.SaveChangesAsync();

            return Ok(stockMovement);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the stock movement.");
        }
    }
}
