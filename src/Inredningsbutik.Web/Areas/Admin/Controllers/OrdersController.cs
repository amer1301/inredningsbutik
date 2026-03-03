using Inredningsbutik.Infrastructure.Data;
using Inredningsbutik.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Inredningsbutik.Web.ViewModels;
using Inredningsbutik.Core.Entities;
using Inredningsbutik.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Inredningsbutik.Infrastructure.Identity;

namespace Inredningsbutik.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AuthPolicies.AdminOnly)]
public class OrdersController : Controller
{
    private readonly AppDbContext _db;
    private readonly IEmailService _emailService;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrdersController(
        AppDbContext db,
        IEmailService emailService,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _emailService = emailService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? status, int page = 1, int pageSize = 25)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 5 or > 200 ? 25 : pageSize;

        var query = _db.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.Product)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(o => o.Status == status);

        query = query.OrderByDescending(o => o.CreatedAt);

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
        var order = await _db.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
            return NotFound();

        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return BadRequest();

        var order = await _db.Orders.FindAsync(id);
        if (order is null)
            return NotFound();

        var newStatus = status.Trim();
        var oldStatus = order.Status;

        if (oldStatus == newStatus)
        {
            TempData["AdminToast"] = "Orderstatus var redan satt.";
            return RedirectToAction(nameof(Details), new { id });
        }

        order.Status = newStatus;
        await _db.SaveChangesAsync();

        try
        {
            var user = await _userManager.FindByIdAsync(order.UserId);

            if (user != null && !string.IsNullOrWhiteSpace(user.Email))
            {
                await _emailService.SendOrderStatusChangedAsync(
                    user.Email,
                    user.UserName ?? "kund",
                    order.Id,
                    newStatus);
            }
        }
        catch
        {

        }

        TempData["AdminToast"] = "Orderstatus uppdaterades.";
        return RedirectToAction(nameof(Details), new { id });
    }
}