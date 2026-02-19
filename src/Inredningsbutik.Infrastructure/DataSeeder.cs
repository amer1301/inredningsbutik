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
                InspirationImageUrls = "/images/inspiration/korplampa2.jpg",
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
            },

            new()
            {
                Name = "Orrefors Graphic Vas rund",
                Description = "Orrefors Graphic är en stilren och stramt utformad vas designad av Magnus Forthmeiier.",
                Price = 499,
                StockQuantity = 20,
                CategoryId = vaser.Id,
                ImageUrl = "/images/products/OrreforsVas-produkt.png",
                HoverImageUrl = "/images/hover/Orrefors-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new()
            {
                Name = "Orrefors Graphic Vas cylinder",
                Description = "Orrefors Graphic är en stilren och stramt utformad vas designad av Magnus Forthmeiier.",
                Price = 599,
                StockQuantity = 20,
                CategoryId = vaser.Id,
                ImageUrl = "/images/products/OrreforsVasHög-produkt.png",
                HoverImageUrl = "/images/hover/Orreforshög-inspiration.jpg",
                CreatedAt = DateTime.UtcNow
            },

            new()
            {
                Name = "Vas nova",
                Description = "Läcker vas av handgjort glas med mönster i ljus beige och vit färg.",
                Price = 249,
                StockQuantity = 20,
                CategoryId = vaser.Id,
                ImageUrl = "/images/products/vasNova-produkt.png",
                HoverImageUrl = "/images/hover/novavas-inspiration.jpg",
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

        // ================================
// SEEDA FAQ (UPSERT)
// ================================
var faq = new List<FaqItem>
{
    // ---------- POPULÄRA ----------
    new() { CategoryKey = "populara", SortOrder = 1, Question = "Jag har precis beställt – vad händer nu?", Answer = "När du lagt din beställning får du en orderbekräftelse via e-post. Därefter plockas ordern på vårt lager och du får ett nytt mail när den är skickad. I leveransmailet finns spårningslänk så du kan följa paketet hela vägen.", IsPublished = true },
    new() { CategoryKey = "populara", SortOrder = 2, Question = "Kan jag ändra eller avboka min order?", Answer = "Vi behandlar ordern snabbt. Hör av dig så fort som möjligt via kundtjänst med ordernummer. Om ordern inte är packad ännu kan vi ofta hjälpa dig att ändra eller avboka.", IsPublished = true },
    new() { CategoryKey = "populara", SortOrder = 3, Question = "Varför saknas en vara i min leverans?", Answer = "Ibland skickas ordern i flera paket om produkter finns på olika lager eller har olika leveranstider. Kontrollera dina leveransmail – där ser du om din order delas upp.", IsPublished = true },
    new() { CategoryKey = "populara", SortOrder = 4, Question = "Var är min beställning? Kan jag spåra den?", Answer = "När din order lämnat vårt lager får du ett leveransmail med spårningslänk. Om du inte hittar mailet, kontrollera skräppost eller logga in och titta under “Mina ordrar”.", IsPublished = true },
    new() { CategoryKey = "populara", SortOrder = 5, Question = "Hur gör jag en retur?", Answer = "Du har normalt 30 dagars returrätt. Kontakta kundtjänst för returinformation och instruktioner. När vi mottagit och godkänt returen återbetalar vi enligt vald betalmetod.", IsPublished = true },
    new() { CategoryKey = "populara", SortOrder = 6, Question = "Jag har ett presentkort – hur använder jag det?", Answer = "Du anger presentkortskoden i kassan. Beloppet dras direkt från totalsumman. Om presentkortet inte täcker hela köpet betalar du resterande med valfri betalmetod.", IsPublished = true },
    new() { CategoryKey = "populara", SortOrder = 7, Question = "Kan jag kombinera rabatter och presentkort?", Answer = "Presentkort kan normalt kombineras med rabatter. Kampanjkoder kan däremot i vissa fall inte kombineras med andra erbjudanden. I kassan ser du om koden godkänns.", IsPublished = true },
    new() { CategoryKey = "populara", SortOrder = 8, Question = "Hur länge är ett presentkort giltigt?", Answer = "Presentkort är giltiga i 24 månader från inköpsdatum, om inget annat anges vid köpet.", IsPublished = true },

    // ---------- LEVERANSER ----------
    new() { CategoryKey = "leveranser", SortOrder = 1, Question = "Vilka leveransalternativ erbjuder ni?", Answer = "Vi erbjuder leverans till ombud samt hemleverans (beroende på postnummer och varornas storlek). Tillgängliga alternativ visas i kassan.", IsPublished = true },
    new() { CategoryKey = "leveranser", SortOrder = 2, Question = "Hur lång är leveranstiden?", Answer = "Normal leveranstid är 1–4 arbetsdagar efter att ordern har skickats. Vid hög belastning eller för beställningsvaror kan det ta längre tid. Aktuell info syns i kassan.", IsPublished = true },
    new() { CategoryKey = "leveranser", SortOrder = 3, Question = "Vad kostar frakten?", Answer = "Fraktkostnaden beror på leveranssätt och orderns storlek. Du ser alltid fraktpriset innan du slutför köpet.", IsPublished = true },
    new() { CategoryKey = "leveranser", SortOrder = 4, Question = "Kan jag ändra leveransadress efter lagd order?", Answer = "Kontakta kundtjänst direkt med ordernummer. Om ordern inte är skickad ännu kan vi ibland ändra adressen.", IsPublished = true },
    new() { CategoryKey = "leveranser", SortOrder = 5, Question = "Paketet är försenat – vad gör jag?", Answer = "Kontrollera spårningen först. Om paketet står stilla i flera dagar eller om du saknar spårningsinfo är du välkommen att kontakta kundtjänst så hjälper vi dig vidare.", IsPublished = true },

    // ---------- RETUR & ÅTERBETALNING ----------
    new() { CategoryKey = "retur", SortOrder = 1, Question = "Hur lång returrätt har jag?", Answer = "Du har normalt 30 dagars returrätt från att du mottagit varan. Produkten ska returneras i väsentligen oförändrat skick.", IsPublished = true },
    new() { CategoryKey = "retur", SortOrder = 2, Question = "När får jag min återbetalning?", Answer = "När vi har mottagit och kontrollerat returen återbetalar vi normalt inom 3–10 bankdagar. Tiden kan variera beroende på betalmetod och bank.", IsPublished = true },
    new() { CategoryKey = "retur", SortOrder = 3, Question = "Vad kostar det att returnera?", Answer = "Returkostnad kan tillkomma beroende på varutyp och returorsak. Kontakta kundtjänst så hjälper vi dig med instruktioner.", IsPublished = true },
    new() { CategoryKey = "retur", SortOrder = 4, Question = "Kan jag byta en vara?", Answer = "Vi erbjuder i första hand retur och ny beställning. Då får du snabbare leverans på rätt produkt och du kan följa processen tydligt.", IsPublished = true },
    new() { CategoryKey = "retur", SortOrder = 5, Question = "Vilka varor kan inte returneras?", Answer = "Specialbeställningar och varor som inte kan säljas igen i originalskick kan undantas. Kontakta oss om du är osäker innan du skickar tillbaka.", IsPublished = true },

    // ---------- REKLAMATION ----------
    new() { CategoryKey = "reklamation", SortOrder = 1, Question = "Min vara är trasig – hur reklamerar jag?", Answer = "Kontakta kundtjänst med ordernummer, beskrivning av felet och gärna bilder. Vi återkommer med nästa steg så snabbt vi kan.", IsPublished = true },
    new() { CategoryKey = "reklamation", SortOrder = 2, Question = "Vad händer efter att jag skickat in en reklamation?", Answer = "Vi bedömer ärendet och kan be om kompletterande information. Om reklamationen godkänns erbjuder vi i regel ersättningsvara, reservdel eller återbetalning.", IsPublished = true },
    new() { CategoryKey = "reklamation", SortOrder = 3, Question = "Måste jag spara originalförpackningen?", Answer = "Det underlättar vid transport och hantering, särskilt för ömtåliga produkter. Om du inte har förpackningen kvar, hör av dig så hjälper vi dig med alternativ.", IsPublished = true },
    new() { CategoryKey = "reklamation", SortOrder = 4, Question = "Hur länge kan jag reklamera?", Answer = "Du har reklamationsrätt enligt konsumentköplagen. Kontakta oss så snart du upptäcker felet.", IsPublished = true },

    // ---------- BETALNING & FAKTURA ----------
    new() { CategoryKey = "betalning", SortOrder = 1, Question = "Vilka betalningsmetoder erbjuder ni?", Answer = "Du kan betala med kort och/eller faktura/delbetalning beroende på vad som är aktiverat i kassan. Exakta alternativ visas när du går till betalning.", IsPublished = true },
    new() { CategoryKey = "betalning", SortOrder = 2, Question = "När dras pengarna från mitt kort?", Answer = "Kortbetalningar reserveras normalt vid köp och dras när ordern behandlas eller skickas, beroende på betalningslösning.", IsPublished = true },
    new() { CategoryKey = "betalning", SortOrder = 3, Question = "Jag har inte fått min faktura – vad gör jag?", Answer = "Kontrollera skräppost och eventuella betalningsappar/portaler. Om du fortfarande inte hittar den, kontakta kundtjänst så hjälper vi dig.", IsPublished = true },
    new() { CategoryKey = "betalning", SortOrder = 4, Question = "Kan jag få en delåterbetalning?", Answer = "Ja, vid retur av en del av ordern återbetalar vi motsvarande belopp. Eventuell frakt återbetalas enligt villkor och returorsak.", IsPublished = true },
};

// UPSERT: lägg till om den inte redan finns (CategoryKey + Question)
foreach (var item in faq)
{
    var exists = await db.FaqItems.AnyAsync(f =>
        f.CategoryKey == item.CategoryKey && f.Question == item.Question);

    if (!exists)
        db.FaqItems.Add(item);
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
