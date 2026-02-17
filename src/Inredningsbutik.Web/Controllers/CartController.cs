using Inredningsbutik.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Inredningsbutik.Web.Controllers;

public class CartController : Controller
{
    private readonly CartService _cart;

    public CartController(CartService cart)
    {
        _cart = cart;
    }

    private bool IsAdminUser =>
        User.Identity?.IsAuthenticated == true && User.IsInRole("Admin");

    private IActionResult BlockAdminCart(string? returnUrl = null)
    {
        TempData["AdminToast"] =
            "Administratörer kan inte handla i butiken. Logga in som kund eller använd Admin-panelen.";

        // Om man kommer från en sida, gå tillbaka dit – annars hem
        if (!string.IsNullOrWhiteSpace(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Index()
    {
        if (IsAdminUser) return BlockAdminCart(Url.Action("Index", "Home"));

        var cart = _cart.GetCart();
        return View(cart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity = 1, string? returnUrl = null)
    {
        if (IsAdminUser) return BlockAdminCart(returnUrl ?? Url.Action("Index", "Home"));

        await _cart.AddAsync(productId, quantity);

        TempData["CartToast"] = "Produkten lades i varukorgen!";
        return Redirect(returnUrl ?? Url.Action("Index", "Cart")!);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int productId)
    {
        if (IsAdminUser) return BlockAdminCart(Url.Action("Index", "Home"));

        _cart.Remove(productId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int productId, int quantity)
    {
        if (IsAdminUser) return BlockAdminCart(Url.Action("Index", "Home"));

        _cart.UpdateQuantity(productId, quantity);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateQuantity([FromBody] UpdateCartDto dto)
    {
        if (IsAdminUser)
            return Forbid(); // 403

        var qty = dto.Quantity < 0 ? 0 : dto.Quantity;

        _cart.UpdateQuantity(dto.ProductId, qty);

        var cart = _cart.GetCart();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);

        var lineTotal = item == null ? 0m : (item.UnitPrice * item.Quantity);

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
