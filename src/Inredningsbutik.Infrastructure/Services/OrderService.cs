using Inredningsbutik.Core.Entities;
using Inredningsbutik.Core.Interfaces;
using Inredningsbutik.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Inredningsbutik.Infrastructure.Services;

public class OrderService
{
    private readonly AppDbContext _db;
    private readonly ILogger<OrderService> _logger;
    private readonly IEmailService _emailService;

    public OrderService(
        AppDbContext db,
        ILogger<OrderService> logger,
        IEmailService emailService)
    {
        _db = db;
        _logger = logger;
        _emailService = emailService;
    }

    public async Task<Order> CreateOrderAsync(
        string userId,
        string customerEmail,
        string customerName,
        List<(int productId, int quantity)> items)
    {
        _logger.LogInformation(
            "Skapar order för userId={UserId}. items={ItemCount}",
            userId, items.Count);

        if (items.Count == 0)
        {
            _logger.LogWarning(
                "Order skapades ej: varukorgen tom. userId={UserId}",
                userId);

            throw new InvalidOperationException("Varukorgen är tom.");
        }

        // Hämta produkterna som beställs
        var productIds = items.Select(i => i.productId).Distinct().ToList();

        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

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
            {
                _logger.LogWarning(
                    "Order skapades ej: ogiltigt antal. userId={UserId}, productId={ProductId}, quantity={Quantity}",
                    userId, productId, quantity);

                throw new InvalidOperationException("Ogiltigt antal.");
            }

            var product = products.SingleOrDefault(p => p.Id == productId);

            if (product is null)
            {
                _logger.LogWarning(
                    "Order skapades ej: produkt saknas. userId={UserId}, productId={ProductId}",
                    userId, productId);

                throw new InvalidOperationException($"Produkten finns inte (Id={productId}).");
            }

            if (product.StockQuantity < quantity)
            {
                _logger.LogWarning(
                    "Order skapades ej: otillräckligt lager. userId={UserId}, productId={ProductId}, requested={Requested}, inStock={InStock}",
                    userId, product.Id, quantity, product.StockQuantity);

                throw new InvalidOperationException(
                    $"Otillräckligt lager för '{product.Name}'.");
            }

            // Minska lager
            product.StockQuantity -= quantity;

            order.OrderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = quantity,
                UnitPrice = product.Price
            });
        }

        order.TotalAmount = order.OrderItems
            .Sum(i => i.UnitPrice * i.Quantity);

        _db.Orders.Add(order);

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Order skapad. orderId={OrderId}, userId={UserId}, total={Total}, rows={RowCount}",
            order.Id, userId, order.TotalAmount, order.OrderItems.Count);

        // Skicka mail – men låt inte hela ordern krascha om mail misslyckas
        try
        {
            await _emailService.SendOrderConfirmationAsync(
                customerEmail,
                customerName,
                order.Id);

            _logger.LogInformation(
                "Bekräftelsemail skickat för orderId={OrderId}",
                order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Misslyckades att skicka mail för orderId={OrderId}",
                order.Id);
        }

        return order;
    }
}