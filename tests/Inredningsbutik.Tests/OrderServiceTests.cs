using Microsoft.Extensions.Logging.Abstractions;
using Inredningsbutik.Core.Entities;
using Inredningsbutik.Infrastructure.Data;
using Inredningsbutik.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Inredningsbutik.Tests;

public class OrderServiceTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateOrderAsync_Beräknar_totalbelopp_korrekt()
    {
        await using var db = CreateDbContext();

        var cat = new Category { Name = "Dekoration", Slug = "dekoration" };
        var p1 = new Product { Name = "Vas", Description = "Test", Price = 100m, StockQuantity = 10, Category = cat, CreatedAt = DateTime.UtcNow };
        var p2 = new Product { Name = "Kudde", Description = "Test", Price = 50m, StockQuantity = 10, Category = cat, CreatedAt = DateTime.UtcNow };

        db.AddRange(cat, p1, p2);
        await db.SaveChangesAsync();

        var service = new OrderService(db, NullLogger<OrderService>.Instance);

        var order = await service.CreateOrderAsync(
            userId: "user-1",
            items: new List<(int productId, int quantity)>
            {
                (p1.Id, 2), // 2 * 100
                (p2.Id, 3)  // 3 * 50
            });

        Assert.Equal(2 * 100m + 3 * 50m, order.TotalAmount);
        Assert.Equal(2, order.OrderItems.Count);
    }

    [Fact]
    public async Task CreateOrderAsync_Minskar_lager_vid_beställning()
    {
        await using var db = CreateDbContext();

        var cat = new Category { Name = "Textil", Slug = "textil" };
        var p = new Product { Name = "Pläd", Description = "Test", Price = 200m, StockQuantity = 5, Category = cat, CreatedAt = DateTime.UtcNow };

        db.AddRange(cat, p);
        await db.SaveChangesAsync();

        var service = new OrderService(db, NullLogger<OrderService>.Instance);

        await service.CreateOrderAsync(
            userId: "user-1",
            items: new List<(int productId, int quantity)>
            {
                (p.Id, 2)
            });

        var updated = await db.Products.SingleAsync(x => x.Id == p.Id);
        Assert.Equal(3, updated.StockQuantity);
    }

    [Fact]
    public async Task CreateOrderAsync_Kastar_fel_om_lager_inte_räcker()
    {
        await using var db = CreateDbContext();

        var cat = new Category { Name = "Belysning", Slug = "belysning" };
        var p = new Product { Name = "Lampa", Description = "Test", Price = 300m, StockQuantity = 1, Category = cat, CreatedAt = DateTime.UtcNow };

        db.AddRange(cat, p);
        await db.SaveChangesAsync();

        var service = new OrderService(db, NullLogger<OrderService>.Instance);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CreateOrderAsync(
                userId: "user-1",
                items: new List<(int productId, int quantity)>
                {
                    (p.Id, 2)
                }));

        Assert.Contains("Otillräckligt lager", ex.Message);
    }
}
