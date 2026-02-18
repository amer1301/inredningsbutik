using Inredningsbutik.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inredningsbutik.Web.Controllers;

public class CatalogController : Controller
{
    private readonly AppDbContext _db;

    public CatalogController(AppDbContext db)
    {
        _db = db;
    }

    // /Catalog eller /Catalog/Index
public async Task<IActionResult> Index(string? q, int? categoryId)
{
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
    {
        query = query.Where(p => p.CategoryId == categoryId.Value);
    }

    var categories = await _db.Categories
        .AsNoTracking()
        .OrderBy(c => c.Name)
        .ToListAsync();

    var products = await query
        .OrderBy(p => p.Name)
        .ToListAsync();

    ViewBag.Categories = categories;
    ViewBag.SelectedCategoryId = categoryId;
    ViewBag.Query = q;

    return View(products);
}


    // /Catalog/Details/5
public async Task<IActionResult> Details(int id)
{
    var product = await _db.Products
        .AsNoTracking()
        .Include(p => p.Category)
        .FirstOrDefaultAsync(p => p.Id == id);

    if (product is null) return NotFound();

    return View(product);
}

}
