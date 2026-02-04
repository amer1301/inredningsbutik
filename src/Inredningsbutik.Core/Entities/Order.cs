namespace Inredningsbutik.Core.Entities;

public class Order
{
    public int Id { get; set; }

    // Kopplas senare till Identity-användare
    public string UserId { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Ny";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<OrderItem> Items { get; set; } = new();
}
