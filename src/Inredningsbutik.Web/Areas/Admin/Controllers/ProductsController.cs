using Inredningsbutik.Infrastructure.Data;
using Inredningsbutik.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Inredningsbutik.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _db.Products
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();

        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        var vm = new ProductFormViewModel
        {
            Categories = await GetCategorySelectListAsync()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Categories = await GetCategorySelectListAsync();
            return View(vm);
        }

        var product = new Inredningsbutik.Core.Entities.Product
        {
            Name = vm.Name,
            Description = vm.Description,
            Price = vm.Price,
            ImageUrl = vm.ImageUrl,
            CategoryId = vm.CategoryId,
            StockQuantity = vm.StockQuantity
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound();

        var vm = new ProductFormViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId,
            StockQuantity = product.StockQuantity,
            Categories = await GetCategorySelectListAsync()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            vm.Categories = await GetCategorySelectListAsync();
            return View(vm);
        }

        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound();

        product.Name = vm.Name;
        product.Description = vm.Description;
        product.Price = vm.Price;
        product.ImageUrl = vm.ImageUrl;
        product.CategoryId = vm.CategoryId;
        product.StockQuantity = vm.StockQuantity;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null) return NotFound();

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound();

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task<List<SelectListItem>> GetCategorySelectListAsync()
    {
        return await _db.Categories
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToListAsync();
    }
}
