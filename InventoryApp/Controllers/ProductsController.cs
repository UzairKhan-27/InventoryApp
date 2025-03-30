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
    public ActionResult<List<Product>> GetProducts()
    {
        var products = dbContext.Products.ToList();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public ActionResult<Product> GetProduct(Guid id)
    {   
        var product = dbContext.Products.Find(id);
        if (product == null)
        {
            return NotFound(id);
        }
        return Ok(product);
    }

    [HttpPost]
    public ActionResult<Product> AddProduct([FromBody] AddProductDto addProductDto)
    {
        var product = new Product()
        {
            Name = addProductDto.Name,
            Price = addProductDto.Price
        };
        dbContext.Products.Add(product);
        dbContext.SaveChanges();
        return Ok(product);
    }

}
