using Inredningsbutik.Infrastructure.Data;
using Inredningsbutik.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Inredningsbutik.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AuthPolicies.AdminOnly)]
public class OrdersController : Controller
{
    private readonly AppDbContext _db;

    public OrdersController(AppDbContext db)
    {
        _db = db;
    }

public async Task<IActionResult> Index(string? status)
{
    var query = _db.Orders
        .AsNoTracking()
        .Include(o => o.OrderItems)
        .ThenInclude(i => i.Product)
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(status))
    {
        query = query.Where(o => o.Status == status);
    }

    var orders = await query
        .OrderByDescending(o => o.CreatedAt)
        .ToListAsync();

    ViewBag.SelectedStatus = status;

    ViewBag.Statuses = await _db.Orders
        .AsNoTracking()
        .Select(o => o.Status)
        .Distinct()
        .OrderBy(s => s)
        .ToListAsync();

    return View(orders);
}


    public async Task<IActionResult> Details(int id)
    {
        var order = await _db.Orders
        .AsNoTracking()
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null) return NotFound();
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return BadRequest();

        var order = await _db.Orders.FindAsync(id);
        if (order is null) return NotFound();

        order.Status = status.Trim();
        await _db.SaveChangesAsync();
        TempData["AdminToast"] = "Orderstatus uppdaterades.";

        return RedirectToAction(nameof(Details), new { id });
    }
}
