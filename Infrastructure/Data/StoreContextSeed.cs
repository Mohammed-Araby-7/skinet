using System.Text.Json;
using Core.Entities;
using Infrastructure.Data;

namespace Infrastructure;


public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext context)
    {
        // if there is not data inside the db
        if(!context.Products.Any())
        {
            var productData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");
            var products = JsonSerializer.Deserialize<List<Product>>(productData);
            if(products == null) return;

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
            
        }
    }
}