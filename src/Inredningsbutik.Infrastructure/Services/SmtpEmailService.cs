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

    public SmtpEmailService(
        IConfiguration config,
        ILogger<SmtpEmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    // ================================
    // ORDERBEKRÄFTELSE
    // ================================
    public async Task SendOrderConfirmationAsync(
        string toEmail,
        string customerName,
        int orderId)
    {
        try
        {
            var subject = $"Orderbekräftelse #{orderId}";

            var body = $@"
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
            </div>";

            await SendEmailAsync(toEmail, subject, body);

            _logger.LogInformation("Orderbekräftelse skickad för order {OrderId}", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Orderbekräftelse misslyckades.");
        }
    }

    // ================================
    // STATUSÄNDRING
    // ================================
    public async Task SendOrderStatusChangedAsync(
        string toEmail,
        string customerName,
        int orderId,
        string newStatus)
    {
        try
        {
            var subject = newStatus == "Skickad"
                ? $"Din order #{orderId} är skickad 📦"
                : $"Din order #{orderId} har uppdaterats";

            var body = newStatus == "Skickad"
                ? $@"
                <h2>Hej {customerName}!</h2>
                <p>Din order <strong>#{orderId}</strong> är nu skickad och på väg till dig.</p>
                <p>Tack för att du handlar hos Design By Persdotter!</p>"
                : $@"
                <h2>Hej {customerName}!</h2>
                <p>Din order <strong>#{orderId}</strong> har ändrats till status:
                <strong>{newStatus}</strong>.</p>";

            await SendEmailAsync(toEmail, subject, body);

            _logger.LogInformation("Statusmail skickat för order {OrderId}", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Statusmail misslyckades.");
        }
    }

    // ================================
    // SUPPORTSVAR
    // ================================
    public async Task SendSupportReplyAsync(
    string toEmail,
    string customerName,
    string replyMessage)
{
    try
    {
        var subject = "Svar på ditt meddelande – Design By Persdotter";

        var body = $@"
        <div style='font-family: Arial, sans-serif; background-color:#f8f8f8; padding:40px;'>
            <div style='max-width:600px; margin:0 auto; background:white; padding:30px; border-radius:8px;'>

                <h1 style='margin-top:0; color:#333;'>Design By Persdotter</h1>

                <h2 style='color:#444;'>Hej {customerName}!</h2>

                <p style='color:#555; font-size:15px;'>
                    Tack för att du kontaktade oss. Här är vårt svar på ditt meddelande:
                </p>

                <div style='background:#f2f2f2; padding:15px; border-radius:6px; margin:20px 0; color:#333;'>
                    {replyMessage}
                </div>

                <p style='color:#555; font-size:15px;'>
                    Om du har fler frågor är du varmt välkommen att svara på detta mail.
                </p>

                <hr style='margin:30px 0;' />

                <p style='font-size:14px; color:#777;'>
                    Vänliga hälsningar,<br/>
                    <strong>Design By Persdotter</strong><br/>
                    Stockholm
                </p>

            </div>
        </div>";

        await SendEmailAsync(toEmail, subject, body);

        _logger.LogInformation("Supportmail skickat till {Email}", toEmail);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Supportmail misslyckades.");
    }
}

    // ================================
    // GEMENSAM SMTP-LOGIK (ENDA STÄLLET SOM SKAPAR SMTPCLIENT)
    // ================================
    private async Task SendEmailAsync(
        
        string toEmail,
        string subject,
        string htmlBody)
    {
        _logger.LogInformation("SMTP METHOD EXECUTED");
        _logger.LogInformation("SMTP CONFIG -> Host: {Host}, Port: {Port}, User: {User}",
    _config["Smtp:Host"],
    _config["Smtp:Port"],
    _config["Smtp:Username"]);
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
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        mail.To.Add(toEmail);

        await client.SendMailAsync(mail);
    }
}