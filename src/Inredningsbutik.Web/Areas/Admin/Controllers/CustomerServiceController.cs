using Inredningsbutik.Core.Entities;
using Inredningsbutik.Infrastructure.Data;
using Inredningsbutik.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inredningsbutik.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CustomerServiceController : Controller
{
    private readonly AppDbContext _db;

    private static readonly string[] StatusOptions = new[]
    {
        "New", "Open", "WaitingCustomer", "Resolved", "Closed"
    };

    public CustomerServiceController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? status)
    {
        var q = _db.SupportTickets.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            q = q.Where(t => t.Status == status);

        var tickets = await q
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync();

        ViewBag.StatusOptions = StatusOptions;
        ViewBag.SelectedStatus = status;

        return View(tickets);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var ticket = await _db.SupportTickets
            .Include(t => t.Replies)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null) return NotFound();

        ViewBag.StatusOptions = StatusOptions;
        return View(ticket);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var ticket = await _db.SupportTickets.FirstOrDefaultAsync(t => t.Id == id);
        if (ticket == null) return NotFound();

        if (!StatusOptions.Contains(status))
            return BadRequest("Ogiltig status");

        ticket.Status = status;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        TempData["AdminToast"] = "Status uppdaterad.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reply(SupportReplyVm vm)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Details), new { id = vm.TicketId });

        var ticket = await _db.SupportTickets.FirstOrDefaultAsync(t => t.Id == vm.TicketId);
        if (ticket == null) return NotFound();

        _db.SupportReplies.Add(new SupportReply
        {
            SupportTicketId = vm.TicketId,
            Message = vm.Message,
            IsAdmin = true,
            CreatedAt = DateTime.UtcNow
        });

        if (ticket.Status == "New")
            ticket.Status = "Open";

        ticket.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        TempData["AdminToast"] = "Svar sparat.";
        return RedirectToAction(nameof(Details), new { id = vm.TicketId });
    }
}
