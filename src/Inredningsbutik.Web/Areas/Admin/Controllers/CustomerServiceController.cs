using Inredningsbutik.Core.Entities;
using Inredningsbutik.Core.Interfaces;
using Inredningsbutik.Infrastructure.Data;
using Inredningsbutik.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inredningsbutik.Infrastructure;
using Inredningsbutik.Web.ViewModels;

namespace Inredningsbutik.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AuthPolicies.AdminOnly)]
public class CustomerServiceController : Controller
{
    private readonly AppDbContext _db;
    private readonly ILogger<CustomerServiceController> _logger;
    private readonly IEmailService _emailService;

    private static readonly string[] StatusOptions = new[]
    {
        "New", "Open", "WaitingCustomer", "Resolved", "Closed"
    };

    public CustomerServiceController(
        AppDbContext db,
        ILogger<CustomerServiceController> logger,
        IEmailService emailService)
    {
        _db = db;
        _logger = logger;
        _emailService = emailService;
    }

    // ================================
    // LISTA ÄRENDEN
    // ================================
    [HttpGet]
    public async Task<IActionResult> Index(string? status, int page = 1, int pageSize = 25)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 5 or > 200 ? 25 : pageSize;

        var q = _db.SupportTickets
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            q = q.Where(t => t.Status == status);

        q = q.OrderByDescending(t => t.UpdatedAt);

        var total = await q.CountAsync();

        var items = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.StatusOptions = StatusOptions;
        ViewBag.SelectedStatus = status;

        var vm = new PagedListVm<SupportTicket>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };

        return View(vm);
    }

    // ================================
    // DETAILS
    // ================================
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var ticket = await _db.SupportTickets
            .Include(t => t.Replies)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null)
            return NotFound();

        ViewBag.StatusOptions = StatusOptions;
        return View(ticket);
    }

    // ================================
    // UPDATE STATUS
    // ================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var ticket = await _db.SupportTickets.FirstOrDefaultAsync(t => t.Id == id);
        if (ticket == null)
            return NotFound();

        if (!StatusOptions.Contains(status))
            return BadRequest("Ogiltig status");

        ticket.Status = status;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        TempData["AdminToast"] = "Status uppdaterad.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // ================================
    // ADMIN SVARAR
    // ================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reply(SupportReplyVm vm)
    {
        var ticket = await _db.SupportTickets
            .Include(t => t.Replies)
            .FirstOrDefaultAsync(t => t.Id == vm.TicketId);

        if (ticket == null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.StatusOptions = StatusOptions;
            return View("Details", ticket);
        }

        // Spara svaret
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

        _logger.LogInformation("Admin svarade på supportärende {TicketId}", vm.TicketId);

        // ================================
        // SKICKA MAIL TILL KUND
        // ================================
        try
        {
            if (!string.IsNullOrWhiteSpace(ticket.Email))
            {
                _logger.LogInformation("Skickar supportmail till {Email}", ticket.Email);

                await _emailService.SendSupportReplyAsync(
                    ticket.Email,
                    ticket.Name ?? "Kund",
                    vm.Message
                );
            }
            else
            {
                _logger.LogWarning("Ingen email hittades på supportärende {TicketId}", vm.TicketId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Misslyckades skicka supportmail för ticket {TicketId}", vm.TicketId);
            // Vi låter inte adminflödet krascha pga mailfel
        }

        TempData["AdminToast"] = "Svar sparat och mail skickat.";
        return RedirectToAction(nameof(Details), new { id = vm.TicketId });
    }
}