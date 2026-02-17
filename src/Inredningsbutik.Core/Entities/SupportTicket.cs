namespace Inredningsbutik.Core.Entities;

public class SupportTicket
{
    public int Id { get; set; }

    public string Email { get; set; } = "";
    public string Name { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Message { get; set; } = "";

    // New / Open / WaitingCustomer / Resolved / Closed
    public string Status { get; set; } = "New";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<SupportReply> Replies { get; set; } = new();
}
