using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Inredningsbutik.Core.Interfaces;

namespace Inredningsbutik.Infrastructure.Services;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IConfiguration config,
        ILogger<SmtpEmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendOrderConfirmationAsync(
        string toEmail,
        string customerName,
        int orderId)
    {
        try
        {
            var host = _config["Smtp:Host"] 
    ?? throw new Exception("SMTP Host saknas");

var portString = _config["Smtp:Port"] 
    ?? throw new Exception("SMTP Port saknas");

var enableSslString = _config["Smtp:EnableSsl"] 
    ?? throw new Exception("SMTP EnableSsl saknas");

var username = _config["Smtp:Username"] 
    ?? throw new Exception("SMTP Username saknas");

var password = _config["Smtp:Password"] 
    ?? throw new Exception("SMTP Password saknas");

var fromEmail = _config["Smtp:FromEmail"] 
    ?? throw new Exception("SMTP FromEmail saknas");

var port = int.Parse(portString);
var enableSsl = bool.Parse(enableSslString);

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl
            };

            var mail = new MailMessage
{
    From = new MailAddress(fromEmail, "Design By Persdotter"),
    Subject = $"Orderbekräftelse #{orderId}",
    IsBodyHtml = true,
    Body = $@"
    <div style='font-family: Arial, sans-serif; background-color:#f8f8f8; padding:40px;'>
        <div style='max-width:600px; margin:0 auto; background:white; padding:30px; border-radius:8px;'>

            <h1 style='margin-top:0; color:#333;'>Design By Persdotter</h1>

            <h2 style='color:#444;'>Tack för din beställning, {customerName}!</h2>

            <p style='color:#555; font-size:15px;'>
                Vi har mottagit din order och den behandlas just nu.
            </p>

            <div style='background:#f2f2f2; padding:15px; border-radius:6px; margin:20px 0;'>
                <strong>Ordernummer:</strong> #{orderId}<br/>
                <strong>Orderdatum:</strong> {DateTime.Now:yyyy-MM-dd}
            </div>

            <p style='color:#555; font-size:15px;'>
                Du får ett nytt mail när din beställning skickas.
            </p>

            <hr style='margin:30px 0;' />

            <p style='font-size:14px; color:#777;'>
                Har du frågor? Svara på detta mail så hjälper vi dig gärna.
            </p>

            <p style='font-size:14px; color:#777; margin-top:30px;'>
                Vänliga hälsningar,<br/>
                <strong>Design By Persdotter</strong><br/>
                Stockholm
            </p>

        </div>
    </div>
    "
};

            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);

            _logger.LogInformation("SMTP-mail skickat för order {OrderId}", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMTP mail misslyckades.");
        }
    }
}