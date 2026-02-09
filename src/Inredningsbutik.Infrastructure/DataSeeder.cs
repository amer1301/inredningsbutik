using Inredningsbutik.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inredningsbutik.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // Kör migrationer automatiskt
        await db.Database.MigrateAsync();

        // ================================
        // SEEDA KATEGORIER
        // ================================

        if (!await db.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Belysning", Slug = "belysning" },
                new Category { Name = "Dekoration", Slug = "dekoration" },
                new Category { Name = "Barn", Slug = "barn" },
                new Category { Name = "Kruka", Slug = "kruka" },
                new Category { Name = "Ljus", Slug = "ljus" },
                new Category { Name = "Ljushållare", Slug = "ljushållare" },
                new Category { Name = "Ljusstake", Slug = "ljusstake" },
                new Category { Name = "Servering", Slug = "servering" },
                new Category { Name = "Vas", Slug = "vas" }
            };

            db.Categories.AddRange(categories);
            await db.SaveChangesAsync();
        }

    // Hämta kategorier från databasen
    var belysning = await db.Categories.FirstAsync(c => c.Slug == "belysning");
    var dekoration = await db.Categories.FirstAsync(c => c.Slug == "dekoration");
    var barn = await db.Categories.FirstAsync(c => c.Slug == "barn");
    var kruka = await db.Categories.FirstAsync(c => c.Slug == "kruka");
    var ljus = await db.Categories.FirstAsync(c => c.Slug == "ljus");
    var ljushållare = await db.Categories.FirstAsync(c => c.Slug == "ljushållare");
    var ljusstake = await db.Categories.FirstAsync(c => c.Slug == "ljusstake");
    var servering = await db.Categories.FirstAsync(c => c.Slug == "servering");
    var vas = await db.Categories.FirstAsync(c => c.Slug == "vas");


        // ================================
        // SEEDA PRODUKTER
        // ================================

        var products = new List<Product>
        {
            new Product
            {
                Name = "Byon dekoration Gorilla",
                Description = "Svart minigorilla från Sagaform AB",
                Price = 149,
                StockQuantity = 20,
                Category = kruka,
                ImageUrl = "/images/products/kruka-apa-produkt.png",
                HoverImageUrl = "/images/hover/kruka-apa-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Kruka, Hjorthuvud",
                Description = "Vacker kruka i form av ett hjorthuvud i en matt svart nyans, från Wikholm Form.",
                Price = 349,
                StockQuantity = 20,
                Category = kruka,
                ImageUrl = "/images/products/kruka-hjort-produkt.png",
                HoverImageUrl = "/images/hover/kruka-hjort-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Kruka, Lejon",
                Description = "Svart kruka i form utav ett lejon, från A Lot Decoration.",
                Price = 529,
                StockQuantity = 20,
                Category = kruka,
                ImageUrl = "/images/products/kruka-lejon-produkt.png",
                HoverImageUrl = "/images/hover/kruka-lejon-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Vas, Big Bouble",
                Description = "Vas för inomhus bruk i modern design, från Skånska Möbelhuset.",
                Price = 249,
                StockQuantity = 20,
                Category = vas,
                ImageUrl = "images/products/bubbelvas-produkt.png",
                HoverImageUrl = "images/hover/bubbelvas-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Karaff 1 L",
                Description = "Paris karaff från Department med ett vackert, karakteristiskt mönster.",
                Price = 899,
                StockQuantity = 20,
                Category = servering,
                ImageUrl = "images/products/karaff-produkt.png",
                HoverImageUrl = "images/hover/karaff-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "BLOOM Saltkar och Sked",
                Description = "Elegant saltkar med sked från Georg Jensen.",
                Price = 399,
                StockQuantity = 20,
                Category = servering,
                ImageUrl = "images/products/georgjensen-produkt.png",
                HoverImageUrl = "images/hover/georgjensen-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Anemone Ljuslykta",
                Description = "Anemone ljuslykta Ø 9cm från Målers Glasbruk.",
                Price = 199,
                StockQuantity = 20,
                Category = ljushållare,
                ImageUrl = "images/products/litenskål-produkt.png",
                HoverImageUrl = "images/hover/litenskål-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Kastehelmi Skål 1.4 L",
                Description = "Iittala Kastehelmi Skål 1,4 L från Iittala.",
                Price = 249,
                StockQuantity = 20,
                Category = servering,
                ImageUrl = "images/products/skål-iittala-produkt.png",
                HoverImageUrl = "images/hover/skål-iittala-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Äggklocka",
                Description = "Katt-timern från Bengt Ek Design.",
                Price = 129,
                StockQuantity = 20,
                Category = servering,
                ImageUrl = "images/products/äggklocka-produkt.png",
                HoverImageUrl = "images/hover/äggklocka-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Glasunderlägg",
                Description = "4 glasunderlägg i metall med pärlemorsbeläggning från Zara Home.",
                Price = 295,
                StockQuantity = 20,
                Category = servering,
                ImageUrl = "images/products/glasunderlägg-produkt.png",
                HoverImageUrl = "images/hover/glasunderlägg-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Värmeljushållare brun",
                Description = "Vacker ljuslykta i glas med lite skiftningar i färgen, från serien Sirius.",
                Price = 159,
                StockQuantity = 20,
                Category = ljushållare,
                ImageUrl = "images/products/ljuslykta-brun-produkt.png",
                HoverImageUrl = "images/hover/ljuslykta-brun-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            
            new Product
            {
                Name = "City light 1, Ljushållare",
                Description = "City Light 1 ljuslykta från Louise Roe.",
                Price = 499,
                StockQuantity = 20,
                Category = ljushållare,
                ImageUrl = "images/products/ljuslykta-liten-produkt.png",
                HoverImageUrl = "images/hover/ljuslykta-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            
            new Product
            {
                Name = "City light 2, Ljushållare",
                Description = "City Light 2 ljuslykta från Louise Roe.",
                Price = 549,
                StockQuantity = 20,
                Category = ljushållare,
                ImageUrl = "images/products/ljuslykta-mellan-produkt.png",
                HoverImageUrl = "images/hover/ljuslykta-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            
            new Product
            {
                Name = "City light 3, Ljushållare",
                Description = "City Light 3 ljuslykta från Louise Roe.",
                Price = 629,
                StockQuantity = 20,
                Category = ljushållare,
                ImageUrl = "images/products/ljuslykta-hög-produkt.png",
                HoverImageUrl = "images/hover/ljuslykta-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Stående elefant, ljus",
                Description = "Ljus i form av en elefant, från Zara Home.",
                Price = 229,
                StockQuantity = 20,
                Category = ljus,
                ImageUrl = "images/products/ljus-elefant-produkt.png",
                HoverImageUrl = "images/hover/ljus-elefant-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Sittande elefant, ljus",
                Description = "Ljus i form av en liten sittande elefant, från Zara Home.",
                Price = 129,
                StockQuantity = 20,
                Category = ljus,
                ImageUrl = "images/products/sittandeelefant-produkt.png",
                HoverImageUrl = "images/hover/sittandeelefant-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Kvinnokropp, ljus",
                Description = "Ljus i form av en kvinnokropp.",
                Price = 129,
                StockQuantity = 20,
                Category = ljus,
                ImageUrl = "images/products/ljus-kvinna-produkt.png",
                HoverImageUrl = "images/hover/ljus-kvinna-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Kub, ljus",
                Description = "Ljus i form av en kub.",
                Price = 119,
                StockQuantity = 20,
                Category = ljus,
                ImageUrl = "images/products/ljus-kub-produkt.png",
                HoverImageUrl = "images/hover/ljus-kub-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Väggljusstake",
                Description = "Bruket väggljusstake, från Storefactory.",
                Price = 599,
                StockQuantity = 20,
                Category = ljusstake,
                ImageUrl = "images/products/väggljusstake-produkt.png",
                HoverImageUrl = "images/hover/väggljusstake-inspiration1.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Cobra Ljusstake",
                Description = "Georg Jensen Cobra Ljusstake, rostfri.",
                Price = 1150,
                StockQuantity = 20,
                Category = ljusstake,
                ImageUrl = "images/products/ljusstake-georgjensen-produkt.png",
                HoverImageUrl = "images/hover/ljusstake-georgjensen-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Carat Ljusstake",
                Description = "Ljusstake från Orrefors i serien Carat.",
                Price = 899,
                StockQuantity = 20,
                Category = ljusstake,
                ImageUrl = "images/products/kristalljusstake-produkt.png",
                HoverImageUrl = "images/hover/kristalljusstake-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Ljusstake, Hjort",
                Description = "2-pack ljusstake i form av rådjur, från Zeline.",
                Price = 249,
                StockQuantity = 20,
                Category = ljusstake,
                ImageUrl = "images/products/ljusstake-hjort-produkt.png",
                HoverImageUrl = "images/hover/ljusstake-hjort-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Knot Table Skulptur",
                Description = "Knot Table inredningsdetalj från Cooee Design.",
                Price = 259,
                StockQuantity = 20,
                Category = dekoration,
                ImageUrl = "images/products/knut-produkt.png",
                HoverImageUrl = "images/hover/knut-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Svart dekorativ låda",
                Description = "Svart dekorativ låda för förvaring eller som presentförpackning.",
                Price = 59,
                StockQuantity = 20,
                Category = dekoration,
                ImageUrl = "images/products/låda-produkt.png",
                HoverImageUrl = "images/hover/låda-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Vit glaspumpa, stor",
                Description = "Dekorativ glaspumpa från Cooee Design.",
                Price = 229,
                StockQuantity = 20,
                Category = dekoration,
                ImageUrl = "images/products/pumpa1-produkt.png",
                HoverImageUrl = "images/hover/pumpa1-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Vit glaspumpa, liten",
                Description = "Dekorativ glaspumpa från Cooee Design.",
                Price = 199,
                StockQuantity = 20,
                Category = dekoration,
                ImageUrl = "images/products/pumpa2-produkt.png",
                HoverImageUrl = "images/hover/pumpa2-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Väggdekoration, Papegoja",
                Description = "Väggdekoration PARROT av polyresin, från Jotex.",
                Price = 349,
                StockQuantity = 20,
                Category = dekoration,
                ImageUrl = "images/products/väggdekor-fågel-produkt.png",
                HoverImageUrl = "images/hover/väggdekor-fågel-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Paraply för barn",
                Description = "Paraply i creme med allover-tryck av orange havtorn med gröna blad från That´s Mine.",
                Price = 239,
                StockQuantity = 20,
                Category = barn,
                ImageUrl = "images/products/paraply-produkt.png",
                HoverImageUrl = "images/hover/paraply-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Ryggsäck för barn",
                Description = "Liten ryggsäck, tillverkad i återvunnen polyester från That´s Mine.",
                Price = 399,
                StockQuantity = 20,
                Category = barn,
                ImageUrl = "images/products/ryggsäck-produkt.png",
                HoverImageUrl = "images/hover/ryggsäck-inspiration1.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Vägglampa, Papegoja",
                Description = "Vägglampa i form av en papegoja från A lot decoration.",
                Price = 1099,
                StockQuantity = 20,
                Category = belysning,
                ImageUrl = "images/products/fågellampa-produkt.png",
                HoverImageUrl = "images/hover/fågellampa-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Bordslampa, Korp",
                Description = "Bordslampa i form av en korp, designad av Marcantonio Raimondi Malerba för designföretaget Seletti.",
                Price = 1499,
                StockQuantity = 20,
                Category = belysning,
                ImageUrl = "images/products/bordslampa-korp-produkt.png",
                HoverImageUrl = "images/hover/bordslampa-korp-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new Product
            {
                Name = "Bordslampa, Omega",
                Description = "Bordslampa Omega, från By Rydéns.",
                Price = 999,
                StockQuantity = 20,
                Category = belysning,
                ImageUrl = "images/products/bordslampa-produkt.png",
                HoverImageUrl = "images/hover/bordslampa-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            }
        };

        // Lägg bara till produkter som inte redan finns
        foreach (var product in products)
        {
            bool exists = await db.Products
                .AnyAsync(p => p.Name == product.Name);

            if (!exists)
            {
                db.Products.Add(product);
            }
        }

        await db.SaveChangesAsync();
    }
}
