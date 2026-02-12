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
        // SEEDA KATEGORIER (UPSERT)
        // ================================
        var categories = new List<Category>
        {
            new() { Name = "Belysning",    Slug = "belysning" },
            new() { Name = "Dekorationer", Slug = "dekorationer" },
            new() { Name = "Barn",         Slug = "barn" },
            new() { Name = "Krukor",       Slug = "krukor" },
            new() { Name = "Ljus",         Slug = "ljus" },
            new() { Name = "Ljushållare",  Slug = "ljushallare" },
            new() { Name = "Ljusstakar",   Slug = "ljusstakar" },
            new() { Name = "Servering",    Slug = "servering" },
            new() { Name = "Vaser",        Slug = "vaser" }
        };

        foreach (var c in categories)
        {
            var existing = await db.Categories.SingleOrDefaultAsync(x => x.Slug == c.Slug);
            if (existing is null)
            {
                db.Categories.Add(c);
            }
            else
            {
                existing.Name = c.Name;
            }
        }

        await db.SaveChangesAsync();

        // ================================
        // HÄMTA KATEGORIER FRÅN DB
        // ================================
        var belysning   = await GetCat(db, "belysning");
        var dekoration  = await GetCat(db, "dekorationer");
        var barn        = await GetCat(db, "barn");
        var krukor      = await GetCat(db, "krukor");
        var ljus        = await GetCat(db, "ljus");
        var ljushallare = await GetCat(db, "ljushallare");
        var ljusstakar  = await GetCat(db, "ljusstakar");
        var servering   = await GetCat(db, "servering");
        var vaser       = await GetCat(db, "vaser");

        // ================================
        // SEEDA PRODUKTER (UPSERT PÅ NAME)
        // ================================
        var products = new List<Product>
        {
            new()
            {
                Name = "Byon dekoration Gorilla",
                Description = "Svart minigorilla från Sagaform AB",
                Price = 149,
                StockQuantity = 20,
                CategoryId = krukor.Id,
                ImageUrl = "/images/products/kruka-apa-produkt.png",
                HoverImageUrl = "/images/hover/kruka-apa-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Kruka, Hjorthuvud",
                Description = "Vacker kruka i form av ett hjorthuvud i en matt svart nyans, från Wikholm Form.",
                Price = 349,
                StockQuantity = 20,
                CategoryId = krukor.Id,
                ImageUrl = "/images/products/kruka-hjort-produkt.png",
                HoverImageUrl = "/images/hover/kruka-hjort-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Kruka, Lejon",
                Description = "Svart kruka i form utav ett lejon, från A Lot Decoration.",
                Price = 529,
                StockQuantity = 20,
                CategoryId = krukor.Id,
                ImageUrl = "/images/products/kruka-lejon-produkt.png",
                HoverImageUrl = "/images/hover/kruka-lejon-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Vas, Big Bouble",
                Description = "Vas för inomhus bruk i modern design, från Skånska Möbelhuset.",
                Price = 249,
                StockQuantity = 20,
                CategoryId = vaser.Id,
                ImageUrl = "/images/products/bubbelvas-produkt.png",
                HoverImageUrl = "/images/hover/bubbelvas-inspiration.jpg",
                InspirationImageUrls = "/images/inspiration/bubbelvas2.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Karaff 1 L",
                Description = "Paris karaff från Department med ett vackert, karakteristiskt mönster.",
                Price = 899,
                StockQuantity = 20,
                CategoryId = servering.Id,
                ImageUrl = "/images/products/karaff-produkt.png",
                HoverImageUrl = "/images/hover/karaff-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "BLOOM Saltkar och Sked",
                Description = "Elegant saltkar med sked från Georg Jensen.",
                Price = 399,
                StockQuantity = 20,
                CategoryId = servering.Id,
                ImageUrl = "/images/products/georgjensen-produkt.png",
                HoverImageUrl = "/images/hover/georgjensen-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Anemone Ljuslykta",
                Description = "Anemone ljuslykta Ø 9cm från Målers Glasbruk.",
                Price = 199,
                StockQuantity = 20,
                CategoryId = ljushallare.Id,
                ImageUrl = "/images/products/litenskål-produkt.png",
                HoverImageUrl = "/images/hover/litenskål-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Kastehelmi Skål 1.4 L",
                Description = "Iittala Kastehelmi Skål 1,4 L från Iittala.",
                Price = 249,
                StockQuantity = 20,
                CategoryId = servering.Id,
                ImageUrl = "/images/products/skål-iittala-produkt.png",
                HoverImageUrl = "/images/hover/skål-iittala-inspiration.jpg",
                InspirationImageUrls = "/images/inspiration/inspiration-kök.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Äggklocka",
                Description = "Katt-timern från Bengt Ek Design.",
                Price = 129,
                StockQuantity = 20,
                CategoryId = servering.Id,
                ImageUrl = "/images/products/äggklocka-produkt.png",
                HoverImageUrl = "/images/hover/äggklocka-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Glasunderlägg",
                Description = "4 glasunderlägg i metall med pärlemorsbeläggning från Zara Home.",
                Price = 295,
                StockQuantity = 20,
                CategoryId = servering.Id,
                ImageUrl = "/images/products/glasunderlägg-produkt.png",
                HoverImageUrl = "/images/hover/glasunderlägg-inspiration.jpg",
                InspirationImageUrls = "/images/inspiration/glasunderlägg2.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Värmeljushållare brun",
                Description = "Vacker ljuslykta i glas med lite skiftningar i färgen, från serien Sirius.",
                Price = 159,
                StockQuantity = 20,
                CategoryId = ljushallare.Id,
                ImageUrl = "/images/products/ljuslykta-brun-produkt.png",
                HoverImageUrl = "/images/hover/ljuslykta-brun-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "City light 1, Ljushållare",
                Description = "City Light 1 ljuslykta från Louise Roe.",
                Price = 499,
                StockQuantity = 20,
                CategoryId = ljushallare.Id,
                ImageUrl = "/images/products/ljuslykta-liten-produkt.png",
                HoverImageUrl = "/images/hover/ljuslykta-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "City light 2, Ljushållare",
                Description = "City Light 2 ljuslykta från Louise Roe.",
                Price = 549,
                StockQuantity = 20,
                CategoryId = ljushallare.Id,
                ImageUrl = "/images/products/ljuslykta-mellan-produkt.png",
                HoverImageUrl = "/images/hover/ljuslykta-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "City light 3, Ljushållare",
                Description = "City Light 3 ljuslykta från Louise Roe.",
                Price = 629,
                StockQuantity = 20,
                CategoryId = ljushallare.Id,
                ImageUrl = "/images/products/ljuslykta-hög-produkt.png",
                HoverImageUrl = "/images/hover/ljuslykta-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Stående elefant, ljus",
                Description = "Ljus i form av en elefant, från Zara Home.",
                Price = 229,
                StockQuantity = 20,
                CategoryId = ljus.Id,
                ImageUrl = "/images/products/ljus-elefant-produkt.png",
                HoverImageUrl = "/images/hover/ljus-elefant-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Sittande elefant, ljus",
                Description = "Ljus i form av en liten sittande elefant, från Zara Home.",
                Price = 129,
                StockQuantity = 20,
                CategoryId = ljus.Id,
                ImageUrl = "/images/products/sittandeelefant-produkt.png",
                HoverImageUrl = "/images/hover/sittandeelefant-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Kvinnokropp, ljus",
                Description = "Ljus i form av en kvinnokropp.",
                Price = 129,
                StockQuantity = 20,
                CategoryId = ljus.Id,
                ImageUrl = "/images/products/ljus-kvinna-produkt.png",
                HoverImageUrl = "/images/hover/ljus-kvinna-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Kub, ljus",
                Description = "Ljus i form av en kub.",
                Price = 119,
                StockQuantity = 20,
                CategoryId = ljus.Id,
                ImageUrl = "/images/products/ljus-kub-produkt.png",
                HoverImageUrl = "/images/hover/ljus-kub-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Väggljusstake",
                Description = "Bruket väggljusstake, från Storefactory.",
                Price = 599,
                StockQuantity = 20,
                CategoryId = ljusstakar.Id,
                ImageUrl = "/images/products/väggljusstake-produkt.png",
                HoverImageUrl = "/images/hover/väggljusstake-inspiration1.jpg",
                InspirationImageUrls = "/images/inspiration/väggljusstake2.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Cobra Ljusstake",
                Description = "Georg Jensen Cobra Ljusstake, rostfri.",
                Price = 1150,
                StockQuantity = 20,
                CategoryId = ljusstakar.Id,
                ImageUrl = "/images/products/ljusstake-georgjensen-produkt.png",
                HoverImageUrl = "/images/hover/ljusstake-georgjensen-inspiration.jpg",
                InspirationImageUrls = "/images/inspiration/georgjensen2.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Carat Ljusstake",
                Description = "Ljusstake från Orrefors i serien Carat.",
                Price = 899,
                StockQuantity = 20,
                CategoryId = ljusstakar.Id,
                ImageUrl = "/images/products/kristalljusstake-produkt.png",
                HoverImageUrl = "/images/hover/kristalljusstake-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Ljusstake, Hjort",
                Description = "2-pack ljusstake i form av rådjur, från Zeline.",
                Price = 249,
                StockQuantity = 20,
                CategoryId = ljusstakar.Id,
                ImageUrl = "/images/products/ljusstake-hjort-produkt.png",
                HoverImageUrl = "/images/hover/ljusstake-hjort-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Knot Table Skulptur",
                Description = "Knot Table inredningsdetalj från Cooee Design.",
                Price = 259,
                StockQuantity = 20,
                CategoryId = dekoration.Id,
                ImageUrl = "/images/products/knut-produkt.png",
                HoverImageUrl = "/images/hover/knut-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Svart dekorativ låda",
                Description = "Svart dekorativ låda för förvaring eller som presentförpackning.",
                Price = 59,
                StockQuantity = 20,
                CategoryId = dekoration.Id,
                ImageUrl = "/images/products/låda-produkt.png",
                HoverImageUrl = "/images/hover/låda-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Vit glaspumpa, stor",
                Description = "Dekorativ glaspumpa från Cooee Design.",
                Price = 229,
                StockQuantity = 20,
                CategoryId = dekoration.Id,
                ImageUrl = "/images/products/pumpa1-produkt.png",
                HoverImageUrl = "/images/hover/pumpa1-inspiration.jpg",
                InspirationImageUrls = "/images/inspiration/pumpa2.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Vit glaspumpa, liten",
                Description = "Dekorativ glaspumpa från Cooee Design.",
                Price = 199,
                StockQuantity = 20,
                CategoryId = dekoration.Id,
                ImageUrl = "/images/products/pumpa2-produkt.png",
                HoverImageUrl = "/images/hover/pumpa2-inspiration.jpg",
                InspirationImageUrls = "/images/inspiration/pumpa2.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Väggdekoration, Papegoja",
                Description = "Väggdekoration PARROT av polyresin, från Jotex.",
                Price = 349,
                StockQuantity = 20,
                CategoryId = barn.Id,
                ImageUrl = "/images/products/väggdekor-fågel-produkt.png",
                HoverImageUrl = "/images/hover/väggdekor-fågel-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Paraply för barn",
                Description = "Paraply i creme med allover-tryck av orange havtorn med gröna blad från That´s Mine.",
                Price = 239,
                StockQuantity = 20,
                CategoryId = barn.Id,
                ImageUrl = "/images/products/paraply-produkt.png",
                HoverImageUrl = "/images/hover/paraply-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Ryggsäck för barn",
                Description = "Liten ryggsäck, tillverkad i återvunnen polyester från That´s Mine.",
                Price = 399,
                StockQuantity = 20,
                CategoryId = barn.Id,
                ImageUrl = "/images/products/ryggsäck-produkt.png",
                HoverImageUrl = "/images/hover/ryggsäck-inspiration1.jpg",
                InspirationImageUrls = "/images/inspiration/ryggsäck2.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Vägglampa, Papegoja",
                Description = "Vägglampa i form av en papegoja från A lot decoration.",
                Price = 1099,
                StockQuantity = 20,
                CategoryId = belysning.Id,
                ImageUrl = "/images/products/fågellampa-produkt.png",
                HoverImageUrl = "/images/hover/fågellampa-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Bordslampa, Korp",
                Description = "Bordslampa i form av en korp, designad av Marcantonio Raimondi Malerba för designföretaget Seletti.",
                Price = 1499,
                StockQuantity = 20,
                CategoryId = belysning.Id,
                ImageUrl = "/images/products/bordslampa-korp-produkt.png",
                HoverImageUrl = "/images/hover/bordslampa-korp-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Bordslampa, Omega",
                Description = "Bordslampa Omega, från By Rydéns.",
                Price = 999,
                StockQuantity = 20,
                CategoryId = belysning.Id,
                ImageUrl = "/images/products/bordslampa-produkt.png",
                HoverImageUrl = "/images/hover/bordslampa-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            }
        };

        foreach (var p in products)
        {
            var existing = await db.Products
                .Include(x => x.Category)
                .SingleOrDefaultAsync(x => x.Name == p.Name);

            if (existing is null)
            {
                db.Products.Add(p);
            }
            else
            {
                existing.Description = p.Description;
                existing.Price = p.Price;
                existing.StockQuantity = p.StockQuantity;
                existing.ImageUrl = p.ImageUrl;
                existing.HoverImageUrl = p.HoverImageUrl;
                existing.InspirationImageUrls = p.InspirationImageUrls;

                existing.CategoryId = p.CategoryId;
            }
        }

        await db.SaveChangesAsync();
    }

    private static async Task<Category> GetCat(AppDbContext db, string slug)
    {
        var cat = await db.Categories.SingleOrDefaultAsync(c => c.Slug == slug);
        if (cat is null)
            throw new InvalidOperationException($"Kategori med slug '{slug}' saknas i databasen. Kontrollera seedern.");
        return cat;
    }
}
