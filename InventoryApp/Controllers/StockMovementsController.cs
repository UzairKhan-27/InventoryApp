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

    public StockMovementsController(ProductContext dbContext )
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public ActionResult<List<StockMovement>> GetStockMovements()
    {
        var stockMovements = dbContext.StockMovements.ToList();
        return Ok(stockMovements);
    }
    [HttpGet("{id}")]
    public ActionResult<StockMovement> GetStockMovement(Guid id)
    {
        var stockMovement = dbContext.StockMovements.Find(id);
        if (stockMovement == null)
        {
            return NotFound($"Stock movement with ID {id} not found.");
        }
        return Ok(stockMovement);
    }
    [HttpPost]
    public ActionResult<StockMovement> AddStockMovement([FromBody] AddStockMovementDto addStockMovementDto)
    {
        var stockMovement = new StockMovement()
        {
            ProductId = addStockMovementDto.ProductId,
            Type = addStockMovementDto.Type,
            Count = addStockMovementDto.Count
        };
        dbContext.StockMovements.Add(stockMovement);
        dbContext.SaveChanges();
        return Ok(stockMovement);
    }

}
