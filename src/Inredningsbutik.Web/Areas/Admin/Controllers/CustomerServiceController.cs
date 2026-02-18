using Inredningsbutik.Core.Entities;
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

    private static readonly string[] StatusOptions = new[]
    {
        "New", "Open", "WaitingCustomer", "Resolved", "Closed"
    };

    public CustomerServiceController(AppDbContext db, ILogger<CustomerServiceController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? status, int page = 1, int pageSize = 25)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 5 or > 200 ? 25 : pageSize;

        _logger.LogInformation(
            "Admin listar supportärenden. status={Status}, page={Page}, pageSize={PageSize}",
            status, page, pageSize);

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

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        _logger.LogInformation("Admin öppnar supportärende. ticketId={TicketId}", id);

        var ticket = await _db.SupportTickets
            .AsNoTracking()
            .Include(t => t.Replies)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null)
        {
            _logger.LogWarning("Admin öppnade supportärende som saknas. ticketId={TicketId}", id);
            return NotFound();
        }

        ViewBag.StatusOptions = StatusOptions;
        return View(ticket);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var ticket = await _db.SupportTickets.FirstOrDefaultAsync(t => t.Id == id);
        if (ticket == null)
        {
            _logger.LogWarning("Admin försökte uppdatera status på saknat supportärende. ticketId={TicketId}", id);
            return NotFound();
        }

        if (!StatusOptions.Contains(status))
        {
            _logger.LogWarning("Admin skickade ogiltig supportstatus. ticketId={TicketId}, status={Status}", id, status);
            return BadRequest("Ogiltig status");
        }

        _logger.LogInformation("Admin uppdaterar supportstatus. ticketId={TicketId}, newStatus={Status}", id, status);

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
        var ticket = await _db.SupportTickets
            .Include(t => t.Replies)
            .FirstOrDefaultAsync(t => t.Id == vm.TicketId);

        if (ticket == null)
        {
            _logger.LogWarning("Admin svarade på saknat supportärende. ticketId={TicketId}", vm.TicketId);
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            // Logga inte själva meddelandetexten (kan vara känsligt)
            _logger.LogWarning(
                "Admin misslyckades svara på supportärende pga validering. ticketId={TicketId}, errors={ErrorCount}",
                vm.TicketId, ModelState.ErrorCount);

            ViewBag.StatusOptions = StatusOptions;
            return View("Details", ticket);
        }

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

        _logger.LogInformation("Admin svarade på supportärende. ticketId={TicketId}", vm.TicketId);

        TempData["AdminToast"] = "Svar sparat.";
        return RedirectToAction(nameof(Details), new { id = vm.TicketId });
    }
}
