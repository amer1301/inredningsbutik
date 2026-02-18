using Inredningsbutik.Web.ViewModels;
using Inredningsbutik.Core.Entities;
using Inredningsbutik.Infrastructure.Data;
using Inredningsbutik.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Inredningsbutik.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AuthPolicies.AdminOnly)]
public class ProductsController : Controller
{
    private readonly AppDbContext _db;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(AppDbContext db, ILogger<ProductsController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? q, int? categoryId, int page = 1, int pageSize = 25)
    {
        // guards
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 5 or > 200 ? 25 : pageSize;

        _logger.LogInformation(
            "Admin listar produkter. q={Q}, categoryId={CategoryId}, page={Page}, pageSize={PageSize}",
            q, categoryId, page, pageSize);

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

        // Categories för filter-UI
        var categories = await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();

        ViewBag.Categories = categories;
        ViewBag.SelectedCategoryId = categoryId;
        ViewBag.Query = q;

        // Count innan paging
        var total = await query.CountAsync();

        // Paging
        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var vm = new PagedListVm<Product>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };

        return View(vm);
    }

    public async Task<IActionResult> Create()
    {
        _logger.LogInformation("Admin öppnade skapa-produkt formulär.");
        await PopulateCategoriesAsync();
        return View(new Product { StockQuantity = 0 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product model)
    {
        ValidateProduct(model);
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Admin misslyckades skapa produkt pga validering. errors={ErrorCount}",
                ModelState.ErrorCount);

            await PopulateCategoriesAsync(model.CategoryId);
            return View(model);
        }

        model.Name = model.Name.Trim();
        model.Description = model.Description.Trim();
        model.ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? null : model.ImageUrl.Trim();
        model.HoverImageUrl = string.IsNullOrWhiteSpace(model.HoverImageUrl) ? null : model.HoverImageUrl.Trim();
        model.InspirationImageUrls = string.IsNullOrWhiteSpace(model.InspirationImageUrls) ? null : model.InspirationImageUrls.Trim();

        _db.Products.Add(model);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Admin skapade produkt. productId={ProductId}, name={Name}",
            model.Id, model.Name);

        TempData["AdminToast"] = "Produkten skapades.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null)
        {
            _logger.LogWarning("Admin försökte redigera saknad produkt (GET). productId={ProductId}", id);
            return NotFound();
        }

        await PopulateCategoriesAsync(product.CategoryId);
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product model)
    {
        if (id != model.Id)
        {
            _logger.LogWarning("Admin skickade mismatch id vid edit. routeId={RouteId}, modelId={ModelId}",
                id, model.Id);
            return BadRequest();
        }

        ValidateProduct(model);
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Admin misslyckades uppdatera produkt pga validering. productId={ProductId}, errors={ErrorCount}",
                model.Id, ModelState.ErrorCount);

            await PopulateCategoriesAsync(model.CategoryId);
            return View(model);
        }

        var product = await _db.Products.FindAsync(id);
        if (product is null)
        {
            _logger.LogWarning("Admin försökte uppdatera saknad produkt (POST). productId={ProductId}", id);
            return NotFound();
        }

        product.Name = model.Name.Trim();
        product.Description = model.Description.Trim();
        product.Price = model.Price;
        product.CategoryId = model.CategoryId;
        product.StockQuantity = model.StockQuantity;
        product.ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? null : model.ImageUrl.Trim();
        product.HoverImageUrl = string.IsNullOrWhiteSpace(model.HoverImageUrl) ? null : model.HoverImageUrl.Trim();
        product.InspirationImageUrls = string.IsNullOrWhiteSpace(model.InspirationImageUrls) ? null : model.InspirationImageUrls.Trim();

        await _db.SaveChangesAsync();

        _logger.LogInformation("Admin uppdaterade produkt. productId={ProductId}, name={Name}",
            product.Id, product.Name);

        TempData["AdminToast"] = "Produkten uppdaterades.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            _logger.LogWarning("Admin försökte radera saknad produkt (GET). productId={ProductId}", id);
            return NotFound();
        }

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null)
        {
            _logger.LogWarning("Admin försökte radera saknad produkt (POST). productId={ProductId}", id);
            return NotFound();
        }

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Admin tog bort produkt. productId={ProductId}", id);

        TempData["AdminToast"] = "Produkten togs bort.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateCategoriesAsync(int? selected = null)
    {
        var categories = await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();

        ViewBag.CategoryOptions = new SelectList(
            categories,
            nameof(Category.Id),
            nameof(Category.Name),
            selected
        );
    }

    private void ValidateProduct(Product model)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
            ModelState.AddModelError(nameof(Product.Name), "Namn är obligatoriskt.");

        if (string.IsNullOrWhiteSpace(model.Description))
            ModelState.AddModelError(nameof(Product.Description), "Beskrivning är obligatorisk.");

        if (model.Price < 0)
            ModelState.AddModelError(nameof(Product.Price), "Pris kan inte vara negativt.");

        if (model.StockQuantity < 0)
            ModelState.AddModelError(nameof(Product.StockQuantity), "Lager kan inte vara negativt.");

        if (model.CategoryId <= 0)
            ModelState.AddModelError(nameof(Product.CategoryId), "Kategori är obligatorisk.");
    }
}
