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
    private readonly ProductService _service;

    public ProductsController(ProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetProducts()
    {
        var products = await _service.GetProducts();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(Guid id)
    {
        var product = await _service.GetProduct(id);
        return product == null ? NotFound($"Product with {id} not found") : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> AddProduct([FromBody] AddProductDto dto)
    {
        var product = await _service.AddProduct(dto);
        return Ok(product);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Product>> UpdateProduct(Guid id, [FromBody] UpdateProductDto dto)
    {
        var product = await _service.UpdateProduct(id, dto);
        return product == null ? NotFound($"Product with {id} not found") : Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        var (isSuccess, message) = await _service.DeleteProduct(id);
        return isSuccess ? Ok(message) : BadRequest(message);
    }
}
