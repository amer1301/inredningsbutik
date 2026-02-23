using Inredningsbutik.Core.Interfaces;
using System.Threading.Tasks;

namespace Inredningsbutik.Tests;

public class FakeEmailService : IEmailService
{
    public Task SendOrderConfirmationAsync(string toEmail, string customerName, int orderId)
    {
        // Gör ingenting – vi vill inte skicka riktiga mail i tester
        return Task.CompletedTask;
    }
}