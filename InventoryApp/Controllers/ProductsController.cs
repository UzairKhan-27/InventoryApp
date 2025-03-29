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

}
