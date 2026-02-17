using Inredningsbutik.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Inredningsbutik.Core.Entities;

namespace Inredningsbutik.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
public DbSet<SupportReply> SupportReplies => Set<SupportReply>();
public DbSet<FaqItem> FaqItems => Set<FaqItem>();


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
    .HasMany(o => o.OrderItems)
    .WithOne(oi => oi.Order!)
    .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.OrderItems)
            .WithOne(oi => oi.Product!)
            .HasForeignKey(oi => oi.ProductId);

            modelBuilder.Entity<SupportTicket>()
    .HasMany(t => t.Replies)
    .WithOne(r => r.Ticket!)
    .HasForeignKey(r => r.SupportTicketId)
    .OnDelete(DeleteBehavior.Cascade);

modelBuilder.Entity<FaqItem>()
    .Property(f => f.CategoryKey)
    .HasMaxLength(50);

    }
}
