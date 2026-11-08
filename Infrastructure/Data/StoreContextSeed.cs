using System.Reflection;
using System.Text.Json;
using Core.Entities;
using Infrastructure.Data;

namespace Infrastructure;


public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext context)
    {
       var path = Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            "SeedData"
        );

        // Seed Products
        if (!context.Products.Any())
        {
            var productPath = Path.Combine(path, "products.json");

            var productData = await File.ReadAllTextAsync(productPath);

            var products = JsonSerializer.Deserialize<List<Product>>(productData);

            if (products == null) return;

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }

        // Seed Delivery Methods
        if (!context.DeliveryMethods.Any())
        {
            var deliveryPath = Path.Combine(path, "delivery.json");

            var dmData = await File.ReadAllTextAsync(deliveryPath);

            var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(dmData);

            if (methods == null) return;

            context.DeliveryMethods.AddRange(methods);
            await context.SaveChangesAsync();
        }
    }
}