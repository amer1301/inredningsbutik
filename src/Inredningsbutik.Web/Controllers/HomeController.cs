using Inredningsbutik.Infrastructure.Data;
using Inredningsbutik.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? q, int? categoryId)
    {
        // 1) Kategorier
        var categories = await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();

        // 2) Basquery för PRODUKT-LISTAN
        var query = _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(p =>
                p.Name.Contains(term) ||
                p.Description.Contains(term));
        }

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        // 3) Counts per kategori
        var countsQuery = _db.Products
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            countsQuery = countsQuery.Where(p =>
                p.Name.Contains(term) ||
                p.Description.Contains(term));
        }

        var categoryCounts = await countsQuery
            .GroupBy(p => p.CategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToListAsync();

        var countsByCategoryId = categoryCounts
            .ToDictionary(x => x.CategoryId, x => x.Count);

        var allProductsCount = await countsQuery.CountAsync();

        // 4) Grupplogik
        var groupSpec = new List<(string Title, string[] Names)>
        {
            ("Hem & inredning", new[] { "Belysning", "Dekorationer", "Krukor", "Vaser" }),
            ("Ljus & tillbehör", new[] { "Ljus", "Ljushållare", "Ljusstakar" }),
            ("Barn", new[] { "Barn" }),
        };

        var assigned = new HashSet<int>();

        var grouped = new List<CategoryGroupVm>
        {
            new CategoryGroupVm
            {
                Title = "Kategorier",
                Items =
                [
                    new CategoryItemVm
                    {
                        Id = null,
                        Name = "Visa alla",
                        ProductCount = allProductsCount
                    }
                ]
            }
        };

        foreach (var (title, names) in groupSpec)
        {
            var items = categories
                .Where(c => names.Contains(c.Name))
                .Select(c =>
                {
                    assigned.Add(c.Id);
                    return new CategoryItemVm
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ProductCount = countsByCategoryId.TryGetValue(c.Id, out var cnt) ? cnt : 0
                    };
                })
                .ToList();

            if (items.Count > 0)
            {
                grouped.Add(new CategoryGroupVm
                {
                    Title = title,
                    Items = items
                });
            }
        }

        var unassigned = categories
            .Where(c => !assigned.Contains(c.Id))
            .Select(c => new CategoryItemVm
            {
                Id = c.Id,
                Name = c.Name,
                ProductCount = countsByCategoryId.TryGetValue(c.Id, out var cnt) ? cnt : 0
            })
            .ToList();

        if (unassigned.Count > 0)
        {
            grouped.Add(new CategoryGroupVm
            {
                Title = "Övrigt",
                Items = unassigned
            });
        }

        var vm = new HomeIndexVm
        {
            Q = q,
            CategoryId = categoryId,
            Categories = categories,
            Products = products,
            CategoryGroups = grouped,
            AllProductsCount = allProductsCount
        };

        return View(vm);
    }
}
