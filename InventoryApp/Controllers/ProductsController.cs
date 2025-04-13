using InventoryApp.Data;
using InventoryApp.Helpers;
using InventoryApp.Models;
using InventoryApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductsService _service;
    public ProductsController(IProductsService service)
    {
        _service = service;
    }

    [HttpGet]
    [EnableRateLimiting("ReadPolicy")]
    public async Task<ActionResult<List<Product>>> GetProducts()
    {
        try
        {
            var products = await _service.GetProducts();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while retrieving products.");
        }
    }

    [HttpGet("{id}")]
    [EnableRateLimiting("ReadPolicy")]
    public async Task<ActionResult<Product>> GetProduct(Guid id)
    {
        try
        {
            var product = await _service.GetProduct(id);
            return product == null ? NotFound($"Product with {id} not found") : Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving product with ID {id}.");
        }
    }

    [Authorize(Roles = "CentralAdmin")]
    [EnableRateLimiting("WritePolicy")]
    [HttpPost]
    public async Task<ActionResult<Product>> AddProduct([FromBody] AddProductDto dto)
    {
        try
        {

            var userContext = new UserContext(User);
            string changedBy = userContext.UserId.ToString();
            var product = await _service.AddProduct(dto, changedBy);

            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while adding the product.");
        }
    }

    [Authorize(Roles = "CentralAdmin")]
    [EnableRateLimiting("WritePolicy")]
    [HttpPut("{id}")]
    public async Task<ActionResult<Product>> UpdateProduct(Guid id, [FromBody] UpdateProductDto dto)
    {
        try
        {
            var userContext = new UserContext(User);
            string changedBy = userContext.UserId.ToString();
            var product = await _service.UpdateProduct(id, dto, changedBy);

            return product == null ? NotFound($"Product with {id} not found") : Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating product with ID {id}.");
        }
    }

    [Authorize(Roles = "CentralAdmin")]
    [EnableRateLimiting("WritePolicy")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        try
        {
            var userContext = new UserContext(User);
            string changedBy = userContext.UserId.ToString();
            var (success, message) = await _service.DeleteProduct(id, changedBy);
            return success ? Ok(message) : BadRequest(message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting product with ID {id}.");
        }
    }
}
