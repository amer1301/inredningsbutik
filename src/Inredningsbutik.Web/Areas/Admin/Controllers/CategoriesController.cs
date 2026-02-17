using Inredningsbutik.Core.Entities;
using Inredningsbutik.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inredningsbutik.Infrastructure;

namespace Inredningsbutik.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AuthPolicies.AdminOnly)]
public class CategoriesController : Controller
{
    private readonly AppDbContext _db;

    public CategoriesController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _db.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();
        return View(categories);
    }

    public IActionResult Create() => View(new Category());

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Category model)
{
    if (string.IsNullOrWhiteSpace(model.Name))
    {
        ModelState.AddModelError(nameof(Category.Name), "Namn är obligatoriskt.");
    }

    // Autogenerera slug om tom
    if (string.IsNullOrWhiteSpace(model.Slug) && !string.IsNullOrWhiteSpace(model.Name))
    {
        model.Slug = Slugify(model.Name);

        // Ta bort ev. "required" fel som redan satts av model binding
        ModelState.Remove(nameof(Category.Slug));
        ModelState.Remove("Slug");
        ModelState.Remove("Category.Slug");
    }

    if (!ModelState.IsValid)
    {
        return View(model);
    }

    model.Name = model.Name.Trim();
    model.Slug = (model.Slug ?? "").Trim();

    _db.Categories.Add(model);
    await _db.SaveChangesAsync();

    TempData["AdminToast"] = "Kategorin skapades.";
    return RedirectToAction(nameof(Index));
}


    public async Task<IActionResult> Edit(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category is null) return NotFound();
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category model)
    {
        if (id != model.Id) return BadRequest();

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            ModelState.AddModelError(nameof(Category.Name), "Namn är obligatoriskt.");
        }

        if (string.IsNullOrWhiteSpace(model.Slug))
        {
            model.Slug = Slugify(model.Name);
        }

        if (!ModelState.IsValid) return View(model);

        var category = await _db.Categories.FindAsync(id);
        if (category is null) return NotFound();

        category.Name = model.Name.Trim();
        category.Slug = model.Slug.Trim();

        await _db.SaveChangesAsync();
        TempData["AdminToast"] = "Kategorin uppdaterades.";
        return RedirectToAction(nameof(Index));
    }

    private static string Slugify(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        var s = input.Trim().ToLowerInvariant();
        s = System.Text.RegularExpressions.Regex.Replace(s, "[^a-z0-9åäö]+", "-");
        return s.Trim('-');
    }

    public async Task<IActionResult> Delete(int id)
    {
        var category = await _db.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (category is null) return NotFound();
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _db.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (category is null) return NotFound();

        if (category.Products.Any())
        {
            ModelState.AddModelError("", "Kategorin kan inte tas bort eftersom den har produkter kopplade till sig.");
            return View("Delete", category);
        }

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        TempData["AdminToast"] = "Kategorin togs bort.";
        return RedirectToAction(nameof(Index));
    }
}
