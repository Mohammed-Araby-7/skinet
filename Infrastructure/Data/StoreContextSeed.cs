
using System.Reflection;
using System.Text.Json;
using Core.Enitites;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class StoreContextSeed
{
    public static async Task SeedAsync(
        StoreContext context,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // 1. Ensure Admin role exists
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            var roleResult = await roleManager.CreateAsync(
                new IdentityRole("Admin")
            );

            if (!roleResult.Succeeded)
            {
                throw new Exception(
                    string.Join(", ", roleResult.Errors.Select(e => e.Description))
                );
            }
        }

        // 2. Find Admin user
        var user = await userManager.FindByEmailAsync("admin@test.com");

        // 3. Create Admin user if not exists
        if (user == null)
        {
            user = new AppUser
            {
                UserName = "admin@test.com",
                Email = "admin@test.com",
                FirstName = "Admin",
                LastName = "Admin",
                EmailConfirmed = true
            };

            var userResult = await userManager.CreateAsync(
                user,
                "Pa$$w0rd"
            );

            if (!userResult.Succeeded)
            {
                throw new Exception(
                    string.Join(", ", userResult.Errors.Select(e => e.Description))
                );
            }
        }

        // 4. Ensure Admin user belongs to Admin role
        if (!await userManager.IsInRoleAsync(user, "Admin"))
        {
            var roleResult = await userManager.AddToRoleAsync(user, "Admin");

            if (!roleResult.Succeeded)
            {
                throw new Exception(
                    string.Join(", ", roleResult.Errors.Select(e => e.Description))
                );
            }
        }

        // 5. Seed products and delivery methods
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            "SeedData"
        );

        if (!context.Products.Any())
        {
            var productPath = Path.Combine(path, "products.json");
            var productData = await File.ReadAllTextAsync(productPath);
            var products = JsonSerializer.Deserialize<List<Product>>(productData);

            if (products != null)
            {
                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }

        if (!context.DeliveryMethods.Any())
        {
            var deliveryPath = Path.Combine(path, "delivery.json");
            var dmData = await File.ReadAllTextAsync(deliveryPath);
            var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(dmData);

            if (methods != null)
            {
                context.DeliveryMethods.AddRange(methods);
                await context.SaveChangesAsync();
            }
        }
    }
}
