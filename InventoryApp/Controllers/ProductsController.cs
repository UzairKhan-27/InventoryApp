using InventoryApp.Data;
using InventoryApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ProductContext dbContext;

    public ProductsController(ProductContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetProducts()
    {
        try
        {
            var products = await dbContext.Products.ToListAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving products.");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(Guid id)
    {
        try
        {
            var product = await dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the product.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Product>> AddProduct([FromBody] AddProductDto addProductDto)
    {
        try
        {
            var product = new Product()
            {
                Name = addProductDto.Name,
                Price = addProductDto.Price
            };
            await dbContext.Products.AddAsync(product);
            await dbContext.SaveChangesAsync();
            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the product.");
        }
    }
}
