using Inredningsbutik.Infrastructure.Data;
using Inredningsbutik.Web.Infrastructure;
using Inredningsbutik.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Inredningsbutik.Web.Services;

public class CartService
{
    private const string CartKey = "CART";
    private readonly IHttpContextAccessor _http;
    private readonly AppDbContext _db;

    public CartService(IHttpContextAccessor http, AppDbContext db)
    {
        _http = http;
        _db = db;
    }

    private ISession Session => _http.HttpContext!.Session;

    public Cart GetCart()
        => Session.GetObject<Cart>(CartKey) ?? new Cart();

    private void SaveCart(Cart cart)
        => Session.SetObject(CartKey, cart);

    public async Task AddAsync(int productId, int quantity = 1)
    {
        if (quantity < 1) quantity = 1;

        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product is null) throw new InvalidOperationException("Produkten finns inte.");

        var cart = GetCart();
        var existing = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (existing is null)
        {
            cart.Items.Add(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                UnitPrice = product.Price,
                Quantity = quantity
            });
        }
        else
        {
            existing.Quantity += quantity;
        }

        SaveCart(cart);
    }

    public void Remove(int productId)
    {
        var cart = GetCart();
        cart.Items.RemoveAll(i => i.ProductId == productId);
        SaveCart(cart);
    }

    public void Clear()
    {
        SaveCart(new Cart());
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        var cart = GetCart();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null) return;

        if (quantity <= 0)
            cart.Items.Remove(item);
        else
            item.Quantity = quantity;

        SaveCart(cart);
    }
}
