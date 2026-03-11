using Inredningsbutik.Core.Interfaces;
using System.Threading.Tasks;

namespace Inredningsbutik.Tests;

public class FakeEmailService : IEmailService
{
    public Task SendOrderConfirmationAsync(
        string toEmail,
        string customerName,
        int orderId)
    {
        // Gör ingenting
        return Task.CompletedTask;
    }

    public Task SendOrderStatusChangedAsync(
        string toEmail,
        string customerName,
        int orderId,
        string newStatus)
    {
        // Fake – gör ingenting
        return Task.CompletedTask;
    }

    public Task SendSupportReplyAsync(
        string toEmail,
        string subject,
        string message)
    {
        // Fake – gör ingenting
        return Task.CompletedTask;
    }
}
