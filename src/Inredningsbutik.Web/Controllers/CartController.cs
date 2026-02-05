using Inredningsbutik.Web.Services;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int productId, int quantity)
    {
        _cart.UpdateQuantity(productId, quantity);
        return RedirectToAction(nameof(Index));
    }
}
