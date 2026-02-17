using Inredningsbutik.Core.Entities;
using Inredningsbutik.Infrastructure.Data;
using Inredningsbutik.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inredningsbutik.Web.Controllers;

public class CustomerServiceController : Controller
{
    private readonly AppDbContext _db;

    public CustomerServiceController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vm = new CustomerServiceIndexVm
        {
            FaqByCategory = await LoadFaqAsync()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTicket(CustomerServiceIndexVm vm)
    {
        if (!ModelState.IsValid)
        {
            vm.FaqByCategory = await LoadFaqAsync();
            return View("Index", vm);
        }

        var ticket = new SupportTicket
        {
            Email = (vm.Ticket.Email ?? "").Trim(),
            Name = (vm.Ticket.Name ?? "").Trim(),
            Subject = (vm.Ticket.Subject ?? "").Trim(),
            Message = (vm.Ticket.Message ?? "").Trim(),
            Status = "New",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.SupportTickets.Add(ticket);
        await _db.SaveChangesAsync();

        // Viktigt: använd en publik "toast"-nyckel
        TempData["CustomerServiceToast"] = "Tack! Ditt ärende är skickat.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<Dictionary<string, List<FaqItem>>> LoadFaqAsync()
    {
        var items = await _db.FaqItems
            .Where(f => f.IsPublished)
            .OrderBy(f => f.CategoryKey)
            .ThenBy(f => f.SortOrder)
            .ToListAsync();

        return items
            .GroupBy(i => i.CategoryKey)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}
