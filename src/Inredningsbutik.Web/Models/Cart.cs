namespace Inredningsbutik.Web.Models;

public class Cart
{
    public List<CartItem> Items { get; set; } = new();

    public decimal Total => Items.Sum(i => i.UnitPrice * i.Quantity);
    public int TotalItems => Items.Sum(i => i.Quantity);
}
