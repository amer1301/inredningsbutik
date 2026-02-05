using Inredningsbutik.Core.Entities;
using Inredningsbutik.Infrastructure.Data;
using Inredningsbutik.Infrastructure.Identity;
using Inredningsbutik.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inredningsbutik.Web.Controllers;

[Authorize] // måste vara inloggad för att beställa
public class CheckoutController : Controller
{
    private readonly CartService _cart;
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _users;

    public CheckoutController(CartService cart, AppDbContext db, UserManager<ApplicationUser> users)
    {
        _cart = cart;
        _db = db;
        _users = users;
    }

    public IActionResult Index()
    {
        var cart = _cart.GetCart();
        if (!cart.Items.Any()) return RedirectToAction("Index", "Cart");
        return View(cart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder()
    {
        var cart = _cart.GetCart();
        if (!cart.Items.Any()) return RedirectToAction("Index", "Cart");

        // Ladda produkter från DB för lagerkontroll + pris
        var productIds = cart.Items.Select(i => i.ProductId).ToList();
        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        // Validera lager
        foreach (var item in cart.Items)
        {
            if (!products.TryGetValue(item.ProductId, out var p))
                return BadRequest("En produkt i varukorgen finns inte längre.");

            if (p.StockQuantity < item.Quantity)
            {
                ModelState.AddModelError("", $"Inte tillräckligt lager för {p.Name}. I lager: {p.StockQuantity}.");
                return View("Index", cart);
            }
        }

        var user = await _users.GetUserAsync(User);
        if (user is null) return Challenge();

        using var tx = await _db.Database.BeginTransactionAsync();

        // Skapa order
        var order = new Order
        {
            UserId = user.Id,
            Status = "Ny",
            CreatedAt = DateTime.UtcNow,
            TotalAmount = 0m,
            Items = new List<OrderItem>()
        };

        foreach (var item in cart.Items)
        {
            var p = products[item.ProductId];

            // Minska lager
            p.StockQuantity -= item.Quantity;

            order.Items.Add(new OrderItem
            {
                ProductId = p.Id,
                Quantity = item.Quantity,
                UnitPrice = p.Price
            });
        }

        order.TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        _cart.Clear();

        return RedirectToAction(nameof(Confirmation), new { id = order.Id });
    }

    public async Task<IActionResult> Confirmation(int id)
    {
        var user = await _users.GetUserAsync(User);
        if (user is null) return Challenge();

        var order = await _db.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

        if (order is null) return NotFound();

        return View(order);
    }
}
