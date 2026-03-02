public interface IEmailService
{
    Task SendOrderConfirmationAsync(string toEmail, string customerName, int orderId);

    Task SendOrderStatusChangedAsync(
        string toEmail,
        string customerName,
        int orderId,
        string newStatus);

    Task SendSupportReplyAsync(
        string toEmail,
        string customerName,
        string replyMessage);
}