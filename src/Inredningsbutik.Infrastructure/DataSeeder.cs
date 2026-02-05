using Inredningsbutik.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inredningsbutik.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();

        if (await db.Products.AnyAsync())
            return; // databasen är redan seedad

        // --- Kategorier ---
        var lighting = new Category { Name = "Belysning", Slug = "belysning" };
        var decor = new Category { Name = "Dekoration", Slug = "dekoration" };
        var textile = new Category { Name = "Textil", Slug = "textil" };

        db.Categories.AddRange(lighting, decor, textile);

        // --- Produkter ---
        var products = new List<Product>
        {
            new Product
            {
                Name = "Modern bordslampa",
                Description = "Elegant bordslampa i minimalistisk design.",
                Price = 499,
                StockQuantity = 10,
                Category = lighting,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Taklampa glas",
                Description = "Stilren taklampa med frostat glas.",
                Price = 899,
                StockQuantity = 5,
                Category = lighting,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Keramisk vas",
                Description = "Handgjord vas i nordisk stil.",
                Price = 299,
                StockQuantity = 15,
                Category = decor,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Väggklocka trä",
                Description = "Tystgående väggklocka i ek.",
                Price = 349,
                StockQuantity = 8,
                Category = decor,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Kudde linne",
                Description = "Mjuk kudde i naturligt linne.",
                Price = 199,
                StockQuantity = 20,
                Category = textile,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Pläd ull",
                Description = "Varm pläd i 100% ull.",
                Price = 599,
                StockQuantity = 7,
                Category = textile,
                CreatedAt = DateTime.UtcNow
            }
        };

        db.Products.AddRange(products);

        await db.SaveChangesAsync();
    }
}
