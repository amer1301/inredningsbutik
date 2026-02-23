namespace Inredningsbutik.Core.Interfaces;

public interface IEmailService
{
    Task SendOrderConfirmationAsync(string toEmail, string customerName, int orderId);
}