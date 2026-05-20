using Core.Entities;
using Core.interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepo repo) :ControllerBase
{
    

    [HttpGet]

    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string? brand , string? type,string? sort)
    {
        return Ok(await repo.GetProductsAsync(brand , type , sort));
    }

    [HttpGet("{id:int}")]//api/Product/2

    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repo.GetProductByIdAsync(id);
        if(product == null) return NotFound();

        return product;
    }
    [HttpGet("brands")]

    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        return Ok(await repo.GetBrandsAsync());
    }

    [HttpGet("types")]

    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        return Ok(await repo.GetTypeAsync());
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        repo.AddProduct(product);

        if(await repo.SaveChangeAsync())
        {
            return CreatedAtAction("GetProduct",new {id = product.Id} , product);
        }

        return BadRequest("Problem Creating Product");
    }

    [HttpPut("{id:int}")]

    public async Task<ActionResult> UpdateProduct(int id , Product product)
    {
        if(product.Id !=id || !ProductExists(id))
        {
            return BadRequest("Cannot update this product");
        }

        repo.UpdateProduct(product);
        if(await repo.SaveChangeAsync())
        {
            return NoContent();
        }

        return BadRequest("Problem Updating the Product");
    }

    [HttpDelete("{id:int}")]

    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await repo.GetProductByIdAsync(id);

        if(product == null) return NotFound();

        repo.DeleteProduct(product);

        if(await repo.SaveChangeAsync())
        {
            return NoContent();
        }

       return BadRequest("Problem Deleting the Product");
    }

    private bool ProductExists(int id)
    {
        return repo.ProductExists(id);
    }


}