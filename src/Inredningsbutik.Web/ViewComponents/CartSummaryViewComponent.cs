using Inredningsbutik.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inredningsbutik.Web.ViewComponents;

public class CartSummaryViewComponent : ViewComponent
{
    private readonly CartService _cart;

    public CartSummaryViewComponent(CartService cart)
    {
        _cart = cart;
    }

    public IViewComponentResult Invoke()
    {
        var cart = _cart.GetCart();
        return View(cart);
    }
}
