using Inredningsbutik.Core.Entities;
using Inredningsbutik.Infrastructure.Data;
using Inredningsbutik.Infrastructure.Identity;
using Inredningsbutik.Infrastructure.Services;
using Inredningsbutik.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inredningsbutik.Web.Controllers;

[Authorize(Roles = "Customer")]
public class CheckoutController : Controller
{
    private readonly CartService _cart;
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    private readonly OrderService _orderService;

    public CheckoutController(
        CartService cart,
        AppDbContext db,
        UserManager<ApplicationUser> users,
        OrderService orderService)
    {
        _cart = cart;
        _db = db;
        _users = users;
        _orderService = orderService;
    }

    public IActionResult Index()
    {
        if (IsAdminUser) return BlockAdminCheckout();

        var cart = _cart.GetCart();
        if (!cart.Items.Any())
            return RedirectToAction("Index", "Cart");

        return View(cart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder()
    {
        if (IsAdminUser) return BlockAdminCheckout();

        var cart = _cart.GetCart();
        if (!cart.Items.Any())
            return RedirectToAction("Index", "Cart");

        var user = await _users.GetUserAsync(User);
        if (user is null)
            return Challenge();

        // Förbered items för service-lagret
        var items = cart.Items
            .Select(i => (i.ProductId, i.Quantity))
            .ToList();

        try
        {
            var order = await _orderService.CreateOrderAsync(
                user.Id,
                user.Email ?? throw new InvalidOperationException("User saknar email."),
                user.UserName ?? user.Email!,
                items);

            _cart.Clear();

            return RedirectToAction(nameof(Confirmation), new { id = order.Id });
        }
        catch (InvalidOperationException ex)
        {
            // Ex: lagerproblem, tom kundvagn etc.
            ModelState.AddModelError("", ex.Message);
            return View("Index", cart);
        }
    }

    public async Task<IActionResult> Confirmation(int id)
    {
        var user = await _users.GetUserAsync(User);
        if (user is null)
            return Challenge();

        var order = await _db.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

        if (order is null)
            return NotFound();

        return View(order);
    }

    private bool IsAdminUser =>
        User.Identity?.IsAuthenticated == true && User.IsInRole("Admin");

    private IActionResult BlockAdminCheckout()
    {
        TempData["AdminToast"] =
            "Administratörer kan inte handla i butiken. Logga in som kund.";

        return RedirectToAction("Index", "Home");
    }
}