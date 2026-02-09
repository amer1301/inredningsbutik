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
        var categories = await _db.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();

        var query = _db.Products
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

        var vm = new HomeIndexVm
        {
            Q = q,
            CategoryId = categoryId,
            Categories = categories,
            Products = products
        };

        return View(vm);
    }
}
