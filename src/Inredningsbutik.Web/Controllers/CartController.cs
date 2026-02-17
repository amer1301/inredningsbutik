using Inredningsbutik.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Inredningsbutik.Web.Controllers;

public class CartController : Controller
{
    private readonly CartService _cart;

    public CartController(CartService cart)
    {
        _cart = cart;
    }

    public IActionResult Index()
    {
        var cart = _cart.GetCart();
        return View(cart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity = 1, string? returnUrl = null)
    {
        await _cart.AddAsync(productId, quantity);

        TempData["CartToast"] = "Produkten lades i varukorgen!";
        return Redirect(returnUrl ?? Url.Action("Index", "Cart")!);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int productId)
    {
        _cart.Remove(productId);
        return RedirectToAction(nameof(Index));
    }

    // Behåll gärna denna som fallback (om JS är avstängt eller någon postar ett vanligt form-submit)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int productId, int quantity)
    {
        _cart.UpdateQuantity(productId, quantity);
        return RedirectToAction(nameof(Index));
    }

    // ✅ Ny endpoint för live-uppdatering via fetch (uppdaterar radtotal + totalsumma utan reload)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateQuantity([FromBody] UpdateCartDto dto)
    {
        var qty = dto.Quantity < 0 ? 0 : dto.Quantity;

        _cart.UpdateQuantity(dto.ProductId, qty);

        // Hämta uppdaterad kundvagn och beräkna nya värden att skicka tillbaka
        var cart = _cart.GetCart();

        var item = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);

        var lineTotal = item == null
            ? 0m
            : (item.UnitPrice * item.Quantity);

        return Json(new
        {
            lineTotal = lineTotal.ToString("0.00"),
            cartTotal = cart.Total.ToString("0.00"),
            cartCount = cart.Items.Sum(i => i.Quantity)
        });
    }

    public class UpdateCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
