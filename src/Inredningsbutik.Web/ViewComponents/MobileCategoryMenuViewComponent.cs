using Inredningsbutik.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inredningsbutik.Web.ViewComponents;

/// <summary>
/// Mobil meny för kategorier (visas i navbar-togglen på små skärmar).
/// Håller sig enkel: listar "Visa alla" + alla kategorier.
/// </summary>
public class MobileCategoryMenuViewComponent : ViewComponent
{
    private readonly AppDbContext _db;

    public MobileCategoryMenuViewComponent(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IViewComponentResult> InvokeAsync(int? categoryId, string? q)
    {
        var categories = await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new MobileCategoryItem(c.Id, c.Name))
            .ToListAsync();

        var vm = new MobileCategoryMenuVm
        {
            CategoryId = categoryId,
            Q = q ?? string.Empty,
            Categories = categories
        };

        return View(vm);
    }
}

public sealed class MobileCategoryMenuVm
{
    public int? CategoryId { get; set; }
    public string Q { get; set; } = string.Empty;
    public List<MobileCategoryItem> Categories { get; set; } = new();
}

public sealed record MobileCategoryItem(int Id, string Name);
