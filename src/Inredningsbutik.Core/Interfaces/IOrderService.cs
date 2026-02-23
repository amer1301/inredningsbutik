using Inredningsbutik.Core.Entities;

namespace Inredningsbutik.Core.Interfaces;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(
        string userId,
        string customerEmail,
        string customerName,
        List<(int productId, int quantity)> items);
}