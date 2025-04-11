using InventoryApp.Data;
using InventoryApp.Models;
using InventoryApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ProductsService _service;

    public ProductsController(ProductsService service)
    {
        _service = service;
    }

    [HttpGet]
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

    [HttpPost]
    public async Task<ActionResult<Product>> AddProduct([FromBody] AddProductDto dto)
    {
        try
        {
            var product = await _service.AddProduct(dto);
            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while adding the product.");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Product>> UpdateProduct(Guid id, [FromBody] UpdateProductDto dto)
    {
        try
        {
            var product = await _service.UpdateProduct(id, dto);
            return product == null ? NotFound($"Product with {id} not found") : Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating product with ID {id}.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        try
        {
            var (isSuccess, message) = await _service.DeleteProduct(id);
            return isSuccess ? Ok(message) : BadRequest(message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting product with ID {id}.");
        }
    }
}
