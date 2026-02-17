namespace Inredningsbutik.Core.Entities;

public class SupportReply
{
    public int Id { get; set; }

    public int SupportTicketId { get; set; }
    public SupportTicket? Ticket { get; set; }

    public string Message { get; set; } = "";

    // true = admin, false = kund (om du senare vill låta kund svara)
    public bool IsAdmin { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
