using Inredningsbutik.Core.Entities;
using Inredningsbutik.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inredningsbutik.Infrastructure.Services;

public class OrderService
{
    private readonly AppDbContext _db;

    public OrderService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Order> CreateOrderAsync(string userId, List<(int productId, int quantity)> items)
    {
        if (items.Count == 0)
            throw new InvalidOperationException("Varukorgen är tom.");

        // Hämta produkterna som beställs
        var productIds = items.Select(i => i.productId).ToList();
        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        // Bygg order + validera lager
        var order = new Order
        {
            UserId = userId,
            Status = "Ny",
            CreatedAt = DateTime.UtcNow,
            OrderItems = new List<OrderItem>()
        };

        foreach (var (productId, quantity) in items)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("Ogiltigt antal.");

            var product = products.SingleOrDefault(p => p.Id == productId)
                ?? throw new InvalidOperationException($"Produkten finns inte (Id={productId}).");

            if (product.StockQuantity < quantity)
                throw new InvalidOperationException($"Otillräckligt lager för '{product.Name}'.");

            // minska lager
            product.StockQuantity -= quantity;

            // rad
            order.OrderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = quantity,
                UnitPrice = product.Price
            });
        }

        order.TotalAmount = order.OrderItems.Sum(i => i.UnitPrice * i.Quantity);

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        return order;
    }
}
