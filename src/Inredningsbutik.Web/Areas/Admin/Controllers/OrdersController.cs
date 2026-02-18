using Inredningsbutik.Infrastructure.Data;
using Inredningsbutik.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Inredningsbutik.Web.ViewModels;
using Inredningsbutik.Core.Entities;

namespace Inredningsbutik.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AuthPolicies.AdminOnly)]
public class OrdersController : Controller
{
    private readonly AppDbContext _db;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(AppDbContext db, ILogger<OrdersController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? status, int page = 1, int pageSize = 25)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 5 or > 200 ? 25 : pageSize;

        _logger.LogInformation("Admin listar ordrar. status={Status}, page={Page}, pageSize={PageSize}",
            status, page, pageSize);

        var query = _db.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.Product)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(o => o.Status == status);
        }

        query = query.OrderByDescending(o => o.CreatedAt);

        // count på filtrerad query (innan paging)
        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.SelectedStatus = status;

        ViewBag.Statuses = await _db.Orders
            .AsNoTracking()
            .Select(o => o.Status)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();

        var vm = new PagedListVm<Order>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        _logger.LogInformation("Admin öppnar orderdetaljer. orderId={OrderId}", id);

        var order = await _db.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
        {
            _logger.LogWarning("Admin öppnade order som saknas. orderId={OrderId}", id);
            return NotFound();
        }

        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            _logger.LogWarning("Admin skickade tom status vid UpdateStatus. orderId={OrderId}", id);
            return BadRequest();
        }

        _logger.LogInformation("Admin uppdaterar orderstatus. orderId={OrderId}, newStatus={Status}",
            id, status);

        var order = await _db.Orders.FindAsync(id);
        if (order is null)
        {
            _logger.LogWarning("Admin försökte uppdatera status på saknad order. orderId={OrderId}", id);
            return NotFound();
        }

        order.Status = status.Trim();
        await _db.SaveChangesAsync();

        TempData["AdminToast"] = "Orderstatus uppdaterades.";

        return RedirectToAction(nameof(Details), new { id });
    }
}
