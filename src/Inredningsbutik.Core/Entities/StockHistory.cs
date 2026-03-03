namespace Inredningsbutik.Core.Entities;

public class StockHistory
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public Product? Product { get; set; }

    public int Change { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string AdminEmail { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}