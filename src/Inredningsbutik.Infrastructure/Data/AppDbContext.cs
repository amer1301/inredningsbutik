using Inredningsbutik.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inredningsbutik.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Pris-precision (viktigt för SQL Server)
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2);

        // Relationer
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category!)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(oi => oi.Order!)
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.OrderItems)
            .WithOne(oi => oi.Product!)
            .HasForeignKey(oi => oi.ProductId);

        // Seed för att bygga publika sidor snabbt
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Belysning", Slug = "belysning" },
            new Category { Id = 2, Name = "Textil", Slug = "textil" },
            new Category { Id = 3, Name = "Dekoration", Slug = "dekoration" }
        );

        var seedCreatedAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Kruka, lejon",
                Description = "Svart lejonkruka från A Lot Decorations.",
                Price = 529m,
                ImageUrl = null,
                CategoryId = 1,
                StockQuantity = 12,
                CreatedAt = seedCreatedAt
            },
                    new Product
                    {
                        Id = 2,
                        Name = "Vas, bubblor",
                        Description = "Celeste Vas från By On.",
                        Price = 449m,
                        ImageUrl = null,
                        CategoryId = 2,
                        StockQuantity = 30,
                        CreatedAt = seedCreatedAt
                    }
                );
    }
}
