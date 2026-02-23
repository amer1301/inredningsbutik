using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inredningsbutik.Core.Entities;
using Inredningsbutik.Core.Interfaces;
using Inredningsbutik.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Inredningsbutik.Infrastructure.Services;

public class OrderService : IOrderService
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
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId saknas.");

        if (items == null || items.Count == 0)
            throw new InvalidOperationException("Varukorgen är tom.");

        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            _logger.LogInformation(
                "Skapar order för userId={UserId}. Antal rader={ItemCount}",
                userId, items.Count);

            var productIds = items
                .Select(i => i.productId)
                .Distinct()
                .ToList();

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
                    throw new InvalidOperationException("Ogiltigt antal.");

                var product = products.SingleOrDefault(p => p.Id == productId)
                    ?? throw new InvalidOperationException(
                        $"Produkten finns inte (Id={productId}).");

                if (product.StockQuantity < quantity)
                    throw new InvalidOperationException(
                        $"Otillräckligt lager för '{product.Name}'.");

                // Minskar lagret
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
            await transaction.CommitAsync();

            // Skicka mail EFTER commit
            try
            {
                await _emailService.SendOrderConfirmationAsync(
                    customerEmail,
                    customerName,
                    order.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Mail misslyckades för orderId={OrderId}",
                    order.Id);
            }

            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Order skapades inte.");
            await transaction.RollbackAsync();
            throw;
        }
    }
}